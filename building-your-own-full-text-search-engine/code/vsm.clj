(ns vsm.core
  (:require [clojure.string :as string]))

(defn extract-words [s]
  (map (comp string/lower-case string/trim) (string/split s #" ")))

(defn extract-all-terms [contentselector idselector docs]
   (let [find-terms (comp extract-words contentselector)]
     (->> docs
          (map (juxt idselector find-terms))
          (filter (comp (partial < 0) count second)))))

(defn calc-local-term-freqs [docs-with-terms]
  (->> docs-with-terms
       (mapcat (fn [[d terms]]
                      (map (fn [[term count]]
                             [term [d count]])
                           (frequencies terms))))
       (group-by first)
       (map (fn [[term docs]]
              [term (into {} (map second docs))]))
       (into {})))

(defn calc-global-term-freqs [docs-with-terms]
  (->> docs-with-terms
       (mapcat (fn [[_ terms]] (distinct terms)))
       (frequencies)))

(defn calc-inverse-doc-frequency [global-term-freqs num-of-docs term]
  (Math/log (/ num-of-docs (get global-term-freqs term 1))))


(defn calc-doc-weight-vectors [docs-with-terms local-term-freqs inverse-doc-freq]
  (->> docs-with-terms
       (map (fn [[d terms]]
              (let [weights (map #(let [local-term-freq (get-in local-term-freqs [% d])
                                        inverse-doc-freq (inverse-doc-freq %)
                                        weight (* local-term-freq inverse-doc-freq)]
                                    [% weight]) terms)
                    weights (into {} weights)]
                [d weights])))))

(defn calc-query-weight-vector [inverse-doc-freq query-terms]
  (map #(vector % (inverse-doc-freq %)) query-terms))


(defn calc-similarity [query-weights doc-weight-vector]
  (letfn [(sqr [x] (* x x))
          (rss [xs] (->> (map sqr xs)
                         (reduce +)
                         (Math/sqrt)))]
    (let [dot-product (->> query-weights
                           (map (fn [[term qweight]]
                                 (* (get doc-weight-vector term 0) qweight)))
                           (reduce +))
          norms (* (rss (map second query-weights))
                   (rss (map second doc-weight-vector)))]
      (if (> norms 0)
          (/ dot-product norms)
        0))))

;example usage
(def docs ["hello world" "hello simon" "hello hello laura" "hello cat world"])

(def docs-with-terms (extract-all-terms identity identity docs))
(def local-term-freqs (calc-local-term-freqs docs-with-terms))
(def global-term-freqs (calc-global-term-freqs docs-with-terms))
(def inverse-doc-freq (partial calc-inverse-doc-frequency global-term-freqs (count docs)))
(def doc-weight-vectors (calc-doc-weight-vectors docs-with-terms local-term-freqs inverse-doc-freq))

(def query ["cat" "simon" "world"])
(def query-weight-vector (calc-query-weight-vector inverse-doc-freq query))
(def query-similarity (partial calc-similarity query-weight-vector))

(->> doc-weight-vectors
     (map (fn [[d weights]]
            [d (query-similarity weights)]))
     (filter (comp (partial < 0) second))
     (sort-by second >))
from pgmpy.factors.discrete import TabularCPD
from pgmpy.models import BayesianModel
from pgmpy.inference import VariableElimination
from pgmpy.estimators import MaximumLikelihoodEstimator
from pgmpy.factors.discrete import JointProbabilityDistribution as JPD
from pgmpy.estimators import BayesianEstimator
from pgmpy.estimators import BDeuScore, K2Score, BicScore
import pandas as pd

model = BayesianModel([
    ('DressCode', 'Tops'),
    ('DressCode', 'Bottoms'),
    ('DressCode', 'Socks'),
    ('DressCode', 'Shoes'),
    ('Tops', 'Bottoms'),
    ('Bottoms', 'Socks'),
    ('Bottoms', 'Shoes'),
    ('Socks', 'Shoes')
])

model2 = BayesianModel([
    ('DressCode', 'Tops'),
    ('DressCode', 'Bottoms'),
    ('DressCode', 'Socks'),
    ('DressCode', 'Shoes'),
    ('Tops', 'Bottoms'),
    ('Bottoms', 'Socks'),
    ('Socks', 'Shoes')
])

file = 'cpd.txt'
f = open(file, 'w')
data = pd.read_csv('data.csv')

k2 = K2Score(data)
bic = BicScore(data)

model.fit(data, estimator=MaximumLikelihoodEstimator)
infer = VariableElimination(model)
print(model.local_independencies('Bottoms'))
for cpd in model.get_cpds():
    s = ''
    for v in cpd.variables:
        s = s + v + ' '
    f.write(s.rstrip(' '))
    f.write('\n')
    for j in cpd.get_values():
        s = ''
        for i in j:
            s = s + str(i)+' '
        f.write(s.rstrip(' '))
        f.write('\n')
    f.write('Marginal\n')
    m = data.value_counts(cpd.variables[0],sort=False,normalize=True)
    s=''
    for mar in m.values:
        s = s + str(mar) + ' '
    f.write(s.rstrip(' '))
    #data.value_counts(cpd.variables[0]).to_csv('marginal.txt')
    f.write('\n')
f.close()
#print(model.check_model())

bdeu = BDeuScore(data, equivalent_sample_size=5)
k2 = K2Score(data)
bic = BicScore(data)

print(bdeu.score(model))
print(k2.score(model))
print(bic.score(model))

print(bdeu.score(model2))
print(k2.score(model2))
print(bic.score(model2))

from pgmpy.estimators import ExhaustiveSearch

es = ExhaustiveSearch(data, scoring_method=k2)
best_model = es.estimate()
print(best_model.edges())

print("\nAll DAGs by score:")
for score, dag in reversed(es.all_scores()):
    if score>-545:
        print(score, dag.edges())

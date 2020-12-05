from pgmpy.factors.discrete import TabularCPD
from pgmpy.models import BayesianModel
from pgmpy.inference import VariableElimination
from pgmpy.estimators import MaximumLikelihoodEstimator
from pgmpy.factors.discrete import JointProbabilityDistribution as JPD
from pgmpy.estimators import BayesianEstimator
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

file = 'cpd.txt'
f = open(file, 'w')
data = pd.read_csv('data.csv')
print(data.value_counts('Tops'))
model.fit(data, estimator=MaximumLikelihoodEstimator)
infer = VariableElimination(model)
print(model.get_cpds('Shoes'))
for cpd in model.get_cpds():
    print(cpd.get_values())
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
    f.write('\n')
f.close()
#print(model.check_model())

from pgmpy.factors.discrete import TabularCPD
from pgmpy.models import BayesianModel
from pgmpy.inference import VariableElimination
from pgmpy.estimators import MaximumLikelihoodEstimator
from pgmpy.factors.discrete import JointProbabilityDistribution as JPD
from pgmpy.estimators import BayesianEstimator
import pandas as pd

model = BayesianModel([
    ('Dress Code', 'Tops'),
    ('Dress Code', 'Bottoms'),
    ('Dress Code', 'Socks'),
    ('Dress Code', 'Shoes'),
    ('Tops', 'Bottoms'),
    ('Bottoms', 'Socks'),
    ('Bottoms', 'Shoes'),
    ('Socks', 'Shoes')
])

file = 'cpd.txt'
f = open(file, 'w')
data = pd.read_csv('data.csv')
print(data)
model.fit(data, estimator=MaximumLikelihoodEstimator)
infer = VariableElimination(model)
for cpd in model.get_cpds():
    print(cpd.get_values())
    for v in cpd.variables:
        f.write(v+' ')
    f.write('\n')
    for j in cpd.get_values():
        for i in j:
            f.write(str(i)+' ')
        f.write('\n')
#print(model.check_model())

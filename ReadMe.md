
## MEDIÇÃO DE ESTRUTURA - MONTAGEM

##### JUNTAS:

- Posição 17: Preencher com posicionamento do DF1
- Posição 23: Preencher com o posicionamento do DF2

- Posição 48: Liberar juntas de PM (juntas batidas em exesso). Separar por nível de inspeção e tipo de junta. Usar as juntas em 
excesso para preencher com 'AL' , usando como base o resold, do mais antigo para o mais recente.

- Posição 54 e 60: Mesma lógica da posição 48 mas para RX e US.

- Posição 61: Ajustar o ststus da junta conforme a lógica:

Fórmula
~~~javascript
=SE(X4="NA";"00 - Aguardando Fabricação";SE(
X4="X";"01 - Aguardando Recebimento";
SE(X4="x";"02 - Aguardando Programação";
SE(OU(W4="";Q4="");"03 - Aguardando Posicionamento";
SE(AB4="";"04 - Aguardando Acoplamento";
SE(AI4="";"05 - Aguardando Solda";
SE(AM4="";"06 - Aguardando Visual de Solda";
SE(OU(AB4="RP";AM4="RP";AV4="RP";BB4="RP";BH4="RP");"10 - Aguardando Reparo";
SE(AV4="";"07 - Aguardando PM";
SE(BH4="";"08 - Aguardando US";
SE(BB4="";"09 - Aguardando RX";
SE(OU(AB4="RP";AM4="RP";AV4="RP";BB4="RP";BH4="RP");"10 - Aguardando Reparo";"11 - Junta Liberada"))))))))))))
~~~

- Posição 72: Programação de Fitup do DF1
- Posição 73: Programação de Fitup do DF2 (ambos importados na tabela componentes)

- Posição 63: (MedJoint) Retornar a programação de Fitup de maior número, porém se estiver null retornar 9999
	Retornar ProgDF1 se >0 & > ProgDf2 Quando ProgDF2>0
	Se Pancake, Colum ou Brace, sempre ProgDF1




- Posição 62: Ultima programação da peça (último evento, fitup, solda ou end)

- Posição 67: Criterio de meidção da junta (se é medido ou não) Comprimento da junta
	Se Posição 62>0
		Se( Posição 15 = "Coluna" ou "Contraventamento" ou "Pancake" = Comprimento da Junta)
		Se(Tipo de Junta = "Junta de Angulo" & DF1 = "Primária" = 0)
		Demais situações = Comprimento da junta

##### PEÇAS:

- Posição 34: Compara com a medição anterior para ver se a peça está concluiída (CRITÉRIO DE MEDIÇÃO)

- Posição 35: Somases da Posição 67 de Juntas (Comprimento) considerando a posição 63 Medjoint

##### CRITÉRIO DE MEDIÇÃO MODEC:

````
Se Posição 34(Criterio de Medição) = 
    "-" Realiza a soma dos comprimentos
    "Grade de Piso" EAP - Posicionamento 90% e DIM Final 100%
    Se Coluna ou Contraventamento: 50% qualquer avanço e 100% End Med Joint
    "Chapa de Piso" EAP - Qualquer avanço 90% e END 100%
````


##### CRITÉRIO DE MEDIÇÃO SEPETIBA:

````
Se Posição 34(Criterio de Medição) = 
    "Grade de Piso" EAP - Posicionamento 90% e End 100%
````

- Posição 21 a 26: Preencher com % de avanço, considerando os criterios de medição
- Posição 36 a 46: Somases dos eventos para preencher  as posições de 21 a 26

- Posição 47 a 54: %avanço * peso

- Posição 55 Podenrado;
		15% Posicionamento
		25% Acoplamento
		50% Solda
		10% End

- Posição 56: Avanço %acumulado da peça

- Posição 57: Peso ponderado da ultima medição

- Posição 58: Avanço no período

- Posição 59: Medição anterior.
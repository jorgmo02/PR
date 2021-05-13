int: numAceitesV;
int: numAceitesN;
int: numAceitesTotales=numAceitesV+numAceitesA;
int: numMeses=6;
int: maxPrecio;
%una var con lo que produces cada mes puede ser útil
int: VALOR;
int: MAXV;
int: MAXN;
int: MCAP;
int: MaxCA;
array [1..numAceitesTotales,1..numMeses] of par 1..MaxCA: CA;
float: MinD;
float: MaxD;
float: MaxDureza;
int: toneladas;
int: MinB;
array [1..numAceitesTotales]of 1..MaxDureza:durezas; 
array [1..numMeses,1..numAceitesTotales] of par 1..maxPrecio:precios;

array[1..numMeses,1..numAceitesTotales] of var int:Produces;
array[1..numMeses,1..numAceitesTotales] of var int:Compras;
array[1..numMeses,1..numAceitesTotales] of var int:AceiteDisponible;

% 1. El producto está dentro de su dureza
constraint forall(m in 1..numMeses)
  (sum(j in 1..numAceitesTotales)(durezas[j]*AceiteDisponible[m,j])<MaxD /\ sum(j in 1..numAceitesTotales)(durezas[j]*AceiteDisponible[m,j])>MinD);

% 2. Se consigue el beneficio mínimo 
 constraint sum(m in 1..numMeses,a in 1..numAceitesTotales)
   ((Produces[m,a]*VALOR) - (Compras[m,a]*precios[m,a] + CA[m,a]*AceiteDisponible[m,a])) >= MinB;
   
% 3. Al final del año tienen que quedar "toneladas" toneladas de aceite
constraint forall(a in 1..numAceitesTotales) (AceiteDisponible[numMeses,a] = toneladas);

% 4. cada mes nos queda lo que compramos menos lo que producimos
constraint forall(m in 1..numMeses,a in 1..numAceitesTotales) (AceiteDisponible[m,a]=Compras[m,a]-Produces[m,a]);

% 5. cada mes produces como mucho MAXV aceites vegetales
constraint forall(m in 1..numMeses)(sum( a in 1..numAceitesV)(Produces[m,a])<=MAXV);

% 6. cada mes produces como mucho MAXN aceites animales
constraint forall(m in 1..numMeses)(sum( a in numAceitesV..numAceitesN)(Produces[m,a])<=MAXN);
% 7. En ningún mes se supera el almacenamiento disponible
constraint forall(m in 1..numMeses)(sum( a in 1..numAceitesTotales)(AceiteDisponible[m,a])<=MCAP);
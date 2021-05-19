int: numAceitesV;
int: numAceitesN;
int: numAceitesTotales=numAceitesV+numAceitesN;
int: numMeses;
%una var con lo que produces cada mes puede ser útil
int: VALOR;
int: MAXV;
int: MAXN;
array [1..numAceitesTotales] of int: MCAP;
array [1..numMeses,1..numAceitesTotales] of int: CA;
float: MinD;
float: MaxD;
array [1..numAceitesTotales] of int: toneladas;
int: MinB;
float: MaxDurezas;
array [1..numAceitesTotales]of par 1.0..MaxDurezas:durezas; 
array [1..numMeses,1..numAceitesTotales] of par int:precios;
array[1..numMeses,1..numAceitesTotales] of var int:Produces;
array[1..numMeses,1..numAceitesTotales] of var int:Compras;
array[1..numMeses,1..numAceitesTotales] of var int:AceiteDisponible;


constraint assert (numAceitesV>=0, "los aceites vegetales no pueden ser negativos");
constraint assert (numAceitesN>=0, "los aceites animales no pueden ser negativos");
constraint assert (numMeses>0, "los meses no pueden ser negativos");
constraint assert (VALOR>0, "el valor del producto debe ser mayor que 0");
constraint assert (MAXV>0, "el máximo de aceites vegetales debe ser mayor que 0");
constraint assert (MAXN>0, "el máximo de aceites vegetales debe ser mayor que 0");
constraint assert (forall(cap in MCAP)(cap>0), "las capacidades máximas mensuales no pueden ser negativas");
constraint assert (forall(m in 1..numMeses, a in 1..numAceitesTotales)(CA[m,a]>0), "las capacidades mensuales no pueden ser negativas");
constraint assert (MinD>0, "las dureza mínima no puede ser negativa");
constraint assert (MaxD>MinD, "las dureza máxima debe superar a la mínima");
constraint assert (forall(ton in toneladas)(ton>0), "las toneladas finales no pueden ser negativas");
constraint assert (MinB>=0,"No pueden permitirse pérdidas");
constraint assert (forall(dur in durezas)(dur>0), "las durezas de cada aceite no pueden ser negativas");
constraint assert (forall(m in 1..numMeses, a in 1..numAceitesTotales)(precios[m,a]>0), "las precios mensuales de los aceites no pueden ser negativos");


% 1. El producto está dentro de su dureza
constraint forall(m in 1..numMeses)
 (sum(j in 1..numAceitesTotales)(durezas[j]*Produces[m,j]) <= MaxD*(sum(j in 1..numAceitesTotales)(Produces[m,j])) /\ sum(j in 1..numAceitesTotales)
(durezas[j]*Produces[m,j]) >= MinD*(sum(j in 1..numAceitesTotales)(Produces[m,j])));

% 2. Se consigue el beneficio mínimo 
 constraint (sum(m in 1..numMeses,a in 1..numAceitesTotales)
   ((Produces[m,a]*VALOR) - (Compras[m,a]*precios[m,a] + CA[m,a]*AceiteDisponible[m,a]))) >= MinB;
   
% 3. Al final del año tienen que quedar "toneladas" toneladas de aceite
constraint forall(a in 1..numAceitesTotales) (AceiteDisponible[numMeses,a] = toneladas[a]);

% 4. cada mes nos queda lo que compramos menos lo que producimos más lo que teníamos almacenado anteriormente
constraint forall(m in 2..numMeses,a in 1..numAceitesTotales) (AceiteDisponible[m,a]=Compras[m,a]-Produces[m,a] +AceiteDisponible[m-1,a]);
%El primer mes no contamos con lo que teníamos almacenado anteriormente
constraint forall(a in 1..numAceitesTotales) (AceiteDisponible[1,a]=Compras[1,a]-Produces[1,a]);

% 5. cada mes produces como mucho MAXV aceites vegetales
constraint forall(m in 1..numMeses)(
    sum( a in 1..numAceitesV)(Produces[m,a])   <= MAXV
);

% 6. cada mes produces como mucho MAXN aceites animales
constraint forall(m in 1..numMeses)(
    sum(a in numAceitesV..numAceitesV + numAceitesN)(Produces[m,a])  <= MAXN
);

% 7. En ningún mes se supera el almacenamiento disponible para cada aceite
constraint forall(m in 1..numMeses,a in 1..numAceitesTotales)((AceiteDisponible[m,a])<=MCAP[a]);

%8.Las compras tienen que ser positivas o 0
constraint forall(m in 1..numMeses, a in 1..numAceitesTotales)(Compras[m,a]>=0);

%9.Los aceites disponibles tienen que ser positivas o 0
constraint forall(m in 1..numMeses, a in 1..numAceitesTotales)(AceiteDisponible[m,a]>=0);

%10.La producción tiene que ser positiva o 0
constraint forall(m in 1..numMeses, a in 1..numAceitesTotales)(Produces[m,a]>=0);


output [ "Produces:\n" ++ "\(Produces[1..numMeses, 1..numAceitesTotales])\n" ++
         "Almacenas:\n" ++ "\(AceiteDisponible[1..numMeses, 1..numAceitesTotales])\n" ++
         "Compras:\n" ++ "\(Compras[1..numMeses, 1..numAceitesTotales])" ];

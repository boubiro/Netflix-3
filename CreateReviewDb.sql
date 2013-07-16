-- Création de la table comme le fait le wrapper Sqlite-net (sans index pour l'instant -> perf à l'insertion)
CREATE TABLE "Review"(
"MovieId" integer ,
"UserId" integer ,
"Date" datetime ,
"Note" integer );

-- il faut aussi applique les pragma dans le fichier pragma.txt

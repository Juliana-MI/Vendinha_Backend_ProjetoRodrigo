
CREATE DATABASE vendinha_banco;

\c vendinha_banco;

-- Tabela Clientes
CREATE TABLE "Clientes" (
    "Id" SERIAL PRIMARY KEY,
    "NomeCompleto" VARCHAR(100) NOT NULL,
    "Cpf" VARCHAR(11) NOT NULL UNIQUE,
    "DataNascimento" TIMESTAMP NOT NULL,
    "Idade" INTEGER NOT NULL,
    "Email" TEXT,
    "Celular" TEXT,
    "Genero" TEXT,
    "Observacao" TEXT
);

-- Tabela Dividas
CREATE TABLE "Dividas" (
    "Id" SERIAL PRIMARY KEY,
    "ClienteId" INTEGER NOT NULL,
    "Valor" DECIMAL(18,2) NOT NULL,
    "Paga" BOOLEAN NOT NULL DEFAULT FALSE,
    "DataCriacao" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "DataPagamento" TIMESTAMP,
    CONSTRAINT "FK_Dividas_Clientes_ClienteId" FOREIGN KEY ("ClienteId") 
        REFERENCES "Clientes"("Id") ON DELETE CASCADE
);

CREATE INDEX "IX_Clientes_NomeCompleto" ON "Clientes"("NomeCompleto");
CREATE INDEX "IX_Dividas_ClienteId" ON "Dividas"("ClienteId");
CREATE INDEX "IX_Dividas_Paga" ON "Dividas"("Paga");
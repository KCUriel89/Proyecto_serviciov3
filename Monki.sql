CREATE DATABASE Moneki;
GO
USE Moneki;
GO

/* ================= USUARIOS ================= */
CREATE TABLE Usuarios (
    ID_Usuario INT IDENTITY PRIMARY KEY,
    Nombre NVARCHAR(100) NOT NULL,
    ApellidoPaterno NVARCHAR(100) NOT NULL,
    ApellidoMaterno NVARCHAR(100) NOT NULL,
    Email NVARCHAR(150) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(200) NOT NULL,
    Telefono NVARCHAR(20),
    Direccion NVARCHAR(300),
    FechaNacimiento DATE,
    FechaRegistro DATETIME DEFAULT GETDATE()
);

/* ================= ADMINISTRADORES ================= */
CREATE TABLE Administradores (
    ID_Administrador INT IDENTITY PRIMARY KEY,
    Nombre NVARCHAR(100) NOT NULL,
    ApellidoPaterno NVARCHAR(100) NOT NULL,
    ApellidoMaterno NVARCHAR(100) NOT NULL,
    Email NVARCHAR(150) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(200) NOT NULL
);

/* ================= TRABAJADORES ================= */
CREATE TABLE Trabajadores (
    ID_Trabajador INT IDENTITY PRIMARY KEY,
    Nombre NVARCHAR(100) NOT NULL,
    ApellidoPaterno NVARCHAR(100) NOT NULL,
    ApellidoMaterno NVARCHAR(100) NOT NULL,
    Cargo NVARCHAR(100) NOT NULL,
    Departamento NVARCHAR(100),
    Email NVARCHAR(150) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(200) NOT NULL
);

/* ================= TRÁMITES ================= */
CREATE TABLE Tramites (
    ID_Tramite INT IDENTITY PRIMARY KEY,
    ID_Usuario INT NOT NULL,
    ID_Trabajador INT NULL,
    TipoTramite VARCHAR(30) NOT NULL, -- INE, TESTAMENTO, SUCESION, COMPRAVENTA
    Estado VARCHAR(30) DEFAULT 'Registrado',
    Observaciones NVARCHAR(500),
    FechaCreacion DATETIME DEFAULT GETDATE(),
    FechaActualizacion DATETIME,

    CONSTRAINT FK_Tramites_Usuarios
        FOREIGN KEY (ID_Usuario) REFERENCES Usuarios(ID_Usuario),

    CONSTRAINT FK_Tramites_Trabajadores
        FOREIGN KEY (ID_Trabajador) REFERENCES Trabajadores(ID_Trabajador)
);

/* ================= INE ================= */
CREATE TABLE TramiteINE (
    ID_INE INT IDENTITY PRIMARY KEY,
    ID_Tramite INT UNIQUE,
    CURP VARCHAR(18),
    ActaNacimiento BIT,
    ComprobanteDomicilio BIT,
    Identificacion BIT,

    FOREIGN KEY (ID_Tramite) REFERENCES Tramites(ID_Tramite)
);

/* ================= TESTAMENTO ================= */
CREATE TABLE TramiteTestamento (
    ID_Testamento INT IDENTITY PRIMARY KEY,
    ID_Tramite INT UNIQUE,
    EstadoCivil VARCHAR(20),
    TieneHijos BIT,
    NumeroHijos INT,
    BienesDeclarados NVARCHAR(500),

    FOREIGN KEY (ID_Tramite) REFERENCES Tramites(ID_Tramite)
);

/* ================= SUCESIÓN ================= */
CREATE TABLE TramiteSucesion (
    ID_Sucesion INT IDENTITY PRIMARY KEY,
    ID_Tramite INT UNIQUE,
    TipoSucesion VARCHAR(20),
    NombreFallecido NVARCHAR(150),
    FechaDefuncion DATE,
    NumeroHerederos INT,

    FOREIGN KEY (ID_Tramite) REFERENCES Tramites(ID_Tramite)
);

/* ================= COMPRAVENTA ================= */
CREATE TABLE TramiteCompraventa (
    ID_Compraventa INT IDENTITY PRIMARY KEY,
    ID_Tramite INT UNIQUE,
    TipoBien VARCHAR(30),
    Valor DECIMAL(12,2),
    Comprador NVARCHAR(150),
    Vendedor NVARCHAR(150),

    FOREIGN KEY (ID_Tramite) REFERENCES Tramites(ID_Tramite)
);

/* ================= DOCUMENTOS ================= */
CREATE TABLE Documentos (
    ID_Documento INT IDENTITY PRIMARY KEY,
    ID_Tramite INT NOT NULL,
    NombreArchivo NVARCHAR(255),
    TipoArchivo NVARCHAR(20),
    RutaArchivo NVARCHAR(500),
    FechaSubida DATETIME DEFAULT GETDATE(),

    FOREIGN KEY (ID_Tramite) REFERENCES Tramites(ID_Tramite)
);

/* ================= BIOMÉTRICOS ================= */
CREATE TABLE Biometricos (
    ID_Biometrico INT IDENTITY PRIMARY KEY,
    ID_Usuario INT NOT NULL,
    TipoDato VARCHAR(30),
    Datos VARBINARY(MAX),
    FechaCaptura DATETIME DEFAULT GETDATE(),

    FOREIGN KEY (ID_Usuario) REFERENCES Usuarios(ID_Usuario)
);

/* ================= CORREOS ================= */
CREATE TABLE CorreosEnviados (
    ID INT IDENTITY PRIMARY KEY,
    Destino NVARCHAR(100),
    Asunto NVARCHAR(200),
    Fecha DATETIME DEFAULT GETDATE()
);

/* ================= RECUPERACIÓN PASSWORD ================= */
CREATE TABLE RecuperacionPassword (
    ID INT IDENTITY PRIMARY KEY,
    Correo NVARCHAR(100),
    Codigo NVARCHAR(10),
    Fecha DATETIME DEFAULT GETDATE(),
    Usado BIT DEFAULT 0
);


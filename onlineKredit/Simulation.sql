use dbLAP_ITIN19

create table tblKreditKarte
(
 IDKreditKarte int not null,
 Inhaber nvarchar(255)  not null,
 Nummer nvarchar(255) not null,
 [GültigBis] smalldatetime not null
)
go

ALTER TABLE tblKreditKarte
ADD
CONSTRAINT PK_KreditKarte
PRIMARY KEY (IDKreditKarte);
GO


ALTER TABLE tblKreditKarte
ADD
CONSTRAINT FK_KreditKarte_Kunde
FOREIGN KEY (IDKreditKarte)
REFERENCES tblKunde(IDKUnde);
GO
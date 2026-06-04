use StoreShoes
go

Create Table Roles (
RolesID int primary key identity(1,1),
RolesName nvarchar(50)
);

Create Table Users (
UsersID int primary key identity(1,1),
RolesID int,
FIO nvarchar(155),
Login nvarchar(155),
Password nvarchar(155),

Foreign key (RolesID) references Roles(RolesID)
);

Create Table Postavshikes (
PostavshikesID int primary key identity(1,1),
PostavshikesName nvarchar(255)
);

Create Table Manufactures (
ManufacturesID int primary key identity(1,1),
ManufacturesName nvarchar(255)
);

Create Table Categories (
CategoriesID int primary key identity(1,1),
CategoriesName nvarchar(255)
);

Create Table Tovars (
TovarsID int primary key identity(1,1) not null,
Article nvarchar(150) not null,
TovarNamesID int,
EdIzmereniya nvarchar(150) not null,
Price int not null,
PostavshikesID int not null,
ManufacturesID int not null,
CategoriesID int not null,
Skidka int not null,
Ostatok int,
Opisanie nvarchar(255),
Images nvarchar(255)

Foreign key (PostavshikesID) references Postavshikes(PostavshikesID),
Foreign key (ManufacturesID) references Manufactures(ManufacturesID),
Foreign key (CategoriesID) references Categories(CategoriesID),
Foreign key (TovarNamesID) references TovarNames(TovarNamesID),
);

Create Table PointVudachies (
PointVudachiesID int primary key identity(1,1),
PointVudachiesName nvarchar(255)
);

Create Table OrderStatuses (
OrderStatusesID int primary key identity(1,1),
OrderStatusesName nvarchar(255)
);

Create Table Orders (
OrdersID int primary key identity(1,1),
DateOrders date,
DateDelivery date,
PointVudachiesID int,
ClientID int,
Code int,
OrderStatusesID int,

Foreign key (PointVudachiesID) references PointVudachies(PointVudachiesID),
Foreign key (ClientID) references Users(UsersID),
Foreign key (OrderStatusesID) references OrderStatuses(OrderStatusesID)
);

Create Table OrderItems (
OrderItemsID int primary key identity(1,1),
OrdersID int,
TovarsID int,
CountTovar int,

Foreign key (OrdersID) references Orders(OrdersID),
Foreign key (TovarsID) references Tovars(TovarsID)
);


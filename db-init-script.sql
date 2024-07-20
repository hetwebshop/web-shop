IF EXISTS (SELECT * FROM sys.databases WHERE name = N'HireMeDB')
BEGIN
    ALTER DATABASE [HireMeDB] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE [HireMeDB];
END
GO

CREATE DATABASE [HireMeDB]
CONTAINMENT = NONE
ON PRIMARY (
    NAME = N'HireMeDB',
    FILENAME = N'/var/opt/mssql/data/HireMeDB.mdf',
    SIZE = 8192KB,
    MAXSIZE = UNLIMITED,
    FILEGROWTH = 65536KB
)
LOG ON (
    NAME = N'HireMeDB_log',
    FILENAME = N'/var/opt/mssql/data/HireMeDB_log.ldf',
    SIZE = 8192KB,
    MAXSIZE = 2048GB,
    FILEGROWTH = 65536KB
)
WITH CATALOG_COLLATION = DATABASE_DEFAULT
GO


USE [HireMeDB]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[__EFMigrationsHistory](
	[MigrationId] [nvarchar](150) NOT NULL,
	[ProductVersion] [nvarchar](32) NOT NULL,
 CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY CLUSTERED 
(
	[MigrationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AdvertisementTypes](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](max) NULL,
 CONSTRAINT [PK_AdvertisementTypes] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ApplicantEducations]    Script Date: 7/20/2024 3:41:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ApplicantEducations](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Degree] [nvarchar](max) NULL,
	[InstitutionName] [nvarchar](max) NULL,
	[FieldOfStudy] [nvarchar](max) NULL,
	[EducationStartYear] [int] NOT NULL,
	[EducationEndYear] [int] NOT NULL,
	[UserJobPostId] [int] NOT NULL,
	[University] [nvarchar](max) NULL,
 CONSTRAINT [PK_ApplicantEducations] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetRoleClaims]    Script Date: 7/20/2024 3:41:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetRoleClaims](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[RoleId] [int] NOT NULL,
	[ClaimType] [nvarchar](max) NULL,
	[ClaimValue] [nvarchar](max) NULL,
 CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetRoles]    Script Date: 7/20/2024 3:41:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetRoles](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](256) NULL,
	[NormalizedName] [nvarchar](256) NULL,
	[ConcurrencyStamp] [nvarchar](max) NULL,
 CONSTRAINT [PK_AspNetRoles] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUserClaims]    Script Date: 7/20/2024 3:41:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserClaims](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[ClaimType] [nvarchar](max) NULL,
	[ClaimValue] [nvarchar](max) NULL,
 CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUserLogins]    Script Date: 7/20/2024 3:41:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserLogins](
	[LoginProvider] [nvarchar](450) NOT NULL,
	[ProviderKey] [nvarchar](450) NOT NULL,
	[ProviderDisplayName] [nvarchar](max) NULL,
	[UserId] [int] NOT NULL,
 CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY CLUSTERED 
(
	[LoginProvider] ASC,
	[ProviderKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUserRoles]    Script Date: 7/20/2024 3:41:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserRoles](
	[UserId] [int] NOT NULL,
	[RoleId] [int] NOT NULL,
 CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUsers]    Script Date: 7/20/2024 3:41:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUsers](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[DateOfBirth] [datetime2](7) NOT NULL,
	[Gender] [int] NOT NULL,
	[LastActive] [datetime2](7) NOT NULL,
	[LastName] [nvarchar](max) NULL,
	[PhotoId] [int] NULL,
	[CityId] [int] NOT NULL,
	[UserName] [nvarchar](256) NULL,
	[NormalizedUserName] [nvarchar](256) NULL,
	[Email] [nvarchar](256) NULL,
	[NormalizedEmail] [nvarchar](256) NULL,
	[EmailConfirmed] [bit] NOT NULL,
	[PasswordHash] [nvarchar](max) NULL,
	[SecurityStamp] [nvarchar](max) NULL,
	[ConcurrencyStamp] [nvarchar](max) NULL,
	[PhoneNumber] [nvarchar](max) NULL,
	[PhoneNumberConfirmed] [bit] NOT NULL,
	[TwoFactorEnabled] [bit] NOT NULL,
	[LockoutEnd] [datetimeoffset](7) NULL,
	[LockoutEnabled] [bit] NOT NULL,
	[AccessFailedCount] [int] NOT NULL,
	[FirstName] [nvarchar](max) NULL,
	[JobCategoryId] [int] NULL,
	[JobTypeId] [int] NULL,
	[Biography] [nvarchar](max) NULL,
	[Position] [nvarchar](max) NULL,
	[CvFilePath] [nvarchar](max) NULL,
	[CompanyId] [int] NULL,
	[IsCompany] [bit] NOT NULL,
 CONSTRAINT [PK_AspNetUsers] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUserTokens]    Script Date: 7/20/2024 3:41:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserTokens](
	[UserId] [int] NOT NULL,
	[LoginProvider] [nvarchar](450) NOT NULL,
	[Name] [nvarchar](450) NOT NULL,
	[Value] [nvarchar](max) NULL,
 CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[LoginProvider] ASC,
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Cities]    Script Date: 7/20/2024 3:41:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Cities](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](max) NULL,
	[PostalCode] [nvarchar](max) NULL,
	[CountryId] [int] NOT NULL,
 CONSTRAINT [PK_Cities] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Companies]    Script Date: 7/20/2024 3:41:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Companies](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CompanyName] [nvarchar](max) NULL,
	[CityId] [int] NOT NULL,
	[Address] [nvarchar](max) NULL,
	[Email] [nvarchar](max) NULL,
	[PhoneNumber] [nvarchar](max) NULL,
	[AboutUs] [nvarchar](max) NULL,
 CONSTRAINT [PK_Companies] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CompanyJobPosts]    Script Date: 7/20/2024 3:41:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CompanyJobPosts](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[JobDescription] [nvarchar](max) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[UpdatedAt] [datetime2](7) NOT NULL,
	[SubmittingUserId] [int] NOT NULL,
	[JobTypeId] [int] NOT NULL,
	[JobCategoryId] [int] NOT NULL,
	[JobPostStatusId] [int] NOT NULL,
	[CityId] [int] NOT NULL,
	[AdStartDate] [datetime2](7) NOT NULL,
	[AdEndDate] [datetime2](7) NOT NULL,
	[AdDuration] [int] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[AdName] [nvarchar](max) NULL,
	[Position] [nvarchar](max) NULL,
	[EmailForReceivingApplications] [nvarchar](max) NULL,
 CONSTRAINT [PK_CompanyJobPosts] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Countries]    Script Date: 7/20/2024 3:41:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Countries](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](max) NULL,
 CONSTRAINT [PK_Countries] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[JobCategories]    Script Date: 7/20/2024 3:41:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[JobCategories](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](max) NULL,
 CONSTRAINT [PK_JobCategories] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[JobPostStatuses]    Script Date: 7/20/2024 3:41:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[JobPostStatuses](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](max) NULL,
 CONSTRAINT [PK_JobPostStatuses] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[JobTypes]    Script Date: 7/20/2024 3:41:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[JobTypes](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](max) NULL,
 CONSTRAINT [PK_JobTypes] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Photos]    Script Date: 7/20/2024 3:41:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Photos](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PublicId] [nvarchar](max) NULL,
	[Url] [nvarchar](max) NULL,
 CONSTRAINT [PK_Photos] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserEducations]    Script Date: 7/20/2024 3:41:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserEducations](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Degree] [nvarchar](max) NULL,
	[University] [nvarchar](max) NULL,
	[InstitutionName] [nvarchar](max) NULL,
	[FieldOfStudy] [nvarchar](max) NULL,
	[EducationStartYear] [int] NOT NULL,
	[EducationEndYear] [int] NOT NULL,
	[UserId] [int] NOT NULL,
 CONSTRAINT [PK_UserEducations] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserJobPosts]    Script Date: 7/20/2024 3:41:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserJobPosts](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Position] [nvarchar](max) NULL,
	[Biography] [nvarchar](max) NULL,
	[Price] [decimal](18, 2) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[UpdatedAt] [datetime2](7) NOT NULL,
	[SubmittingUserId] [int] NOT NULL,
	[JobTypeId] [int] NOT NULL,
	[JobCategoryId] [int] NOT NULL,
	[JobPostStatusId] [int] NOT NULL,
	[ApplicantFirstName] [nvarchar](max) NULL,
	[ApplicantLastName] [nvarchar](max) NULL,
	[ApplicantDateOfBirth] [datetime2](7) NOT NULL,
	[ApplicantEmail] [nvarchar](max) NULL,
	[ApplicantGender] [int] NOT NULL,
	[ApplicantPhoneNumber] [nvarchar](max) NULL,
	[CityId] [int] NOT NULL,
	[AdvertisementTypeId] [int] NOT NULL,
	[CvFilePath] [nvarchar](max) NULL,
	[AdDuration] [int] NOT NULL,
	[AdEndDate] [datetime2](7) NOT NULL,
	[AdStartDate] [datetime2](7) NOT NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_UserJobPosts] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20240316165321_InitialMigration', N'6.0.2')
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20240316182153_UserJobPostInitModels', N'6.0.2')
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20240316182926_DeleteJobSubcategoriesAndPutAllInJobCategories', N'6.0.2')
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20240316184637_FixJobTypeNameColumnType', N'6.0.2')
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20240317211845_ApplicantFNLNInUserJobPost', N'6.0.2')
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20240317214726_RenameTitleWithPosition', N'6.0.2')
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20240319223341_AdditionalUserJobFields', N'6.0.2')
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20240319223716_ConfigureApplicantEducation', N'6.0.2')
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20240319231318_AddUniversityToApplicantEducation', N'6.0.2')
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20240320163325_RemoveCantonAndMunicipality', N'6.0.2')
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20240320201256_AddUserJobCategoriesTable', N'6.0.2')
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20240320201830_RenameUserJobCategoryToSubcategory', N'6.0.2')
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20240320212911_AddAdvertisementTypeTable', N'6.0.2')
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20240320213635_SetAdvertisementTypeRequiredInUserJobPost', N'6.0.2')
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20240320220817_ChangingColumnNames', N'6.0.2')
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20240324180607_RemoveUserAddressTable', N'6.0.2')
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20240324180710_UserCityIdRequired', N'6.0.2')
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20240324182549_AddFNLNToUser', N'6.0.2')
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20240324232410_AddUserEducatins', N'6.0.2')
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20240324232508_RenameUserEducations', N'6.0.2')
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20240702184740_AddPositionAndBiographyToUser', N'6.0.2')
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20240702212124_AddCVFilePath', N'6.0.2')
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20240703092846_AddCvFilePathToUserJobPost', N'6.0.2')
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20240703114400_AdDurationStartEndDateToUserJobPost', N'6.0.2')
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20240703204141_AddIsDeleteToUserJobPost', N'6.0.2')
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20240714154728_AddCompanyTable', N'6.0.2')
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20240714173516_AddAboutUsToCompanyTable', N'6.0.2')
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20240715194014_AddCompanyJobPostTable', N'6.0.2')
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20240715205255_CompanyJobAdNameAndPositionCols', N'6.0.2')
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20240715210728_CompanyJobEmailForReceivingApplications', N'6.0.2')
GO
SET IDENTITY_INSERT [dbo].[AdvertisementTypes] ON 
GO
INSERT [dbo].[AdvertisementTypes] ([Id], [Name]) VALUES (1, N'Oglas za posao')
GO
INSERT [dbo].[AdvertisementTypes] ([Id], [Name]) VALUES (2, N'Usluga')
GO
SET IDENTITY_INSERT [dbo].[AdvertisementTypes] OFF
GO
SET IDENTITY_INSERT [dbo].[ApplicantEducations] ON 
GO
INSERT [dbo].[ApplicantEducations] ([Id], [Degree], [InstitutionName], [FieldOfStudy], [EducationStartYear], [EducationEndYear], [UserJobPostId], [University]) VALUES (1, N'MA', N'Prirodno Matematicki Fakultet', N'Kompjuterske nauke', 2016, 2019, 50, N'Univerzitet u Sarajevu')
GO
INSERT [dbo].[ApplicantEducations] ([Id], [Degree], [InstitutionName], [FieldOfStudy], [EducationStartYear], [EducationEndYear], [UserJobPostId], [University]) VALUES (4, N'ere', N'ere', N'ere', 22, 25, 50, N'rere')
GO
INSERT [dbo].[ApplicantEducations] ([Id], [Degree], [InstitutionName], [FieldOfStudy], [EducationStartYear], [EducationEndYear], [UserJobPostId], [University]) VALUES (1002, N'aa', N'aa', N'aa', 22, 23, 177, N'aa')
GO
INSERT [dbo].[ApplicantEducations] ([Id], [Degree], [InstitutionName], [FieldOfStudy], [EducationStartYear], [EducationEndYear], [UserJobPostId], [University]) VALUES (1003, N'BA', N'PMF', N'MAT', 22, 24, 181, N'UNSA')
GO
INSERT [dbo].[ApplicantEducations] ([Id], [Degree], [InstitutionName], [FieldOfStudy], [EducationStartYear], [EducationEndYear], [UserJobPostId], [University]) VALUES (1004, N'SRSK', N'PGSA', N'MAT', 2015, 2025, 181, N'PGSA')
GO
INSERT [dbo].[ApplicantEducations] ([Id], [Degree], [InstitutionName], [FieldOfStudy], [EducationStartYear], [EducationEndYear], [UserJobPostId], [University]) VALUES (1005, N'nn', N'nn', N'nn', 22, 0, 181, N'nn')
GO
INSERT [dbo].[ApplicantEducations] ([Id], [Degree], [InstitutionName], [FieldOfStudy], [EducationStartYear], [EducationEndYear], [UserJobPostId], [University]) VALUES (1006, N'BA', N'PMF', N'MAT', 22, 24, 182, N'UNSA')
GO
INSERT [dbo].[ApplicantEducations] ([Id], [Degree], [InstitutionName], [FieldOfStudy], [EducationStartYear], [EducationEndYear], [UserJobPostId], [University]) VALUES (1007, N'SRSK', N'PGSA', N'MAT', 2015, 2025, 182, N'PGSA')
GO
INSERT [dbo].[ApplicantEducations] ([Id], [Degree], [InstitutionName], [FieldOfStudy], [EducationStartYear], [EducationEndYear], [UserJobPostId], [University]) VALUES (1008, N'nn', N'nn', N'nn', 22, 0, 182, N'nn')
GO
SET IDENTITY_INSERT [dbo].[ApplicantEducations] OFF
GO
SET IDENTITY_INSERT [dbo].[AspNetRoles] ON 
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [NormalizedName], [ConcurrencyStamp]) VALUES (1, N'Admin', N'ADMIN', NULL)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [NormalizedName], [ConcurrencyStamp]) VALUES (2, N'User', N'USER', NULL)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [NormalizedName], [ConcurrencyStamp]) VALUES (3, N'Company', N'Company', NULL)
GO
SET IDENTITY_INSERT [dbo].[AspNetRoles] OFF
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (59, 2)
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (58, 3)
GO
SET IDENTITY_INSERT [dbo].[AspNetUsers] ON 
GO
INSERT [dbo].[AspNetUsers] ([Id], [DateOfBirth], [Gender], [LastActive], [LastName], [PhotoId], [CityId], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount], [FirstName], [JobCategoryId], [JobTypeId], [Biography], [Position], [CvFilePath], [CompanyId], [IsCompany]) VALUES (1, CAST(N'1996-04-17T00:00:00.0000000' AS DateTime2), 0, CAST(N'2021-06-01T00:00:00.0000000' AS DateTime2), N'Admin', 1, 1, N'admin', N'ADMIN', N'admin@mail.com', N'ADMIN@MAIL.COM', 0, N'AQAAAAEAACcQAAAAEGcvFHPkKEQIyeXxZLgTNLCao4u26S/KGSWXoKffM16f3mEaVkCgeg95o8lkJ69ibg==', N'PQBKJR6LDPDA6PIKGGJD37WRNRBH2UJL', N'994fe72c-39c7-43f6-9ccd-4cfcb1676774', N'+919844288854', 0, 0, NULL, 1, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0)
GO
INSERT [dbo].[AspNetUsers] ([Id], [DateOfBirth], [Gender], [LastActive], [LastName], [PhotoId], [CityId], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount], [FirstName], [JobCategoryId], [JobTypeId], [Biography], [Position], [CvFilePath], [CompanyId], [IsCompany]) VALUES (2, CAST(N'1996-04-17T00:00:00.0000000' AS DateTime2), 0, CAST(N'2021-06-01T00:00:00.0000000' AS DateTime2), N'Test User', 2, 1, N'test_user', N'TEST_USER', N'testuser@mail.com', N'TESTUSER@MAIL.COM', 0, N'AQAAAAEAACcQAAAAECcsYth9IVtDUpbWgfL8EffYr+m3mxbrYmJDkC5pWMohSFbnplya4XKSyFniCOvQKQ==', N'EGF3RBKDHZKSK7JC7GSZTLNFJYWM63OG', N'aa8e3468-639c-4684-9eac-7fa8c66b93f4', N'+919876543210', 0, 0, NULL, 1, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0)
GO
INSERT [dbo].[AspNetUsers] ([Id], [DateOfBirth], [Gender], [LastActive], [LastName], [PhotoId], [CityId], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount], [FirstName], [JobCategoryId], [JobTypeId], [Biography], [Position], [CvFilePath], [CompanyId], [IsCompany]) VALUES (3, CAST(N'1982-03-11T00:00:00.0000000' AS DateTime2), 0, CAST(N'2021-08-16T00:00:00.0000000' AS DateTime2), N'Roronoa Zoro', 3, 1, N'zoro', N'ZORO', N'zoro@mail.com', N'ZORO@MAIL.COM', 0, N'AQAAAAEAACcQAAAAENQ6YdzpfUZBnmkDeif0oVeCIZJmMCL1T7bJdFwZltVT9y5XMRyoRoD8ZuQbyHlD2w==', N'DUJHKA7HSLXO4KJFVOXN422JHT67KHRO', N'102208de-1052-4550-b546-a7f54ed887cb', N'+918046402674', 0, 0, NULL, 1, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0)
GO
INSERT [dbo].[AspNetUsers] ([Id], [DateOfBirth], [Gender], [LastActive], [LastName], [PhotoId], [CityId], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount], [FirstName], [JobCategoryId], [JobTypeId], [Biography], [Position], [CvFilePath], [CompanyId], [IsCompany]) VALUES (4, CAST(N'1992-02-29T00:00:00.0000000' AS DateTime2), 0, CAST(N'2021-04-22T00:00:00.0000000' AS DateTime2), N'Monkey D. Luffy', 4, 1, N'luffy', N'LUFFY', N'luffy@mail.com', N'LUFFY@MAIL.COM', 0, N'AQAAAAEAACcQAAAAEH7ZAguiVv5/u8TD3zbYnKUdHSCPyYOQBcfB185g1m0W6IJYygqknL4oCwOvT5oo+g==', N'JK6ZVEBIQ3BS6V4DTE5KHF36SKYCQ7N4', N'a11da1e0-3ee0-4ac8-a66a-f8116fddf294', N'+919011627143', 0, 0, NULL, 1, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0)
GO
INSERT [dbo].[AspNetUsers] ([Id], [DateOfBirth], [Gender], [LastActive], [LastName], [PhotoId], [CityId], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount], [FirstName], [JobCategoryId], [JobTypeId], [Biography], [Position], [CvFilePath], [CompanyId], [IsCompany]) VALUES (5, CAST(N'1991-08-23T00:00:00.0000000' AS DateTime2), 0, CAST(N'2021-07-25T00:00:00.0000000' AS DateTime2), N'Natsu Dragneel', 5, 1, N'natsu', N'NATSU', N'natsu@mail.com', N'NATSU@MAIL.COM', 0, N'AQAAAAEAACcQAAAAEKLlVMVVkd8+AZucgcrQVeP0Yi4m+KRCPKJSeMfDqwP+kl6lBOoi1wtMytGqhvuMDQ==', N'DQWV6PBQX2VKTZSM2MQN5RYCA4NVJQ27', N'0d1e34d3-5462-445f-803a-71c5976ae8f4', N'+918635624870', 0, 0, NULL, 1, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0)
GO
INSERT [dbo].[AspNetUsers] ([Id], [DateOfBirth], [Gender], [LastActive], [LastName], [PhotoId], [CityId], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount], [FirstName], [JobCategoryId], [JobTypeId], [Biography], [Position], [CvFilePath], [CompanyId], [IsCompany]) VALUES (6, CAST(N'1987-01-15T00:00:00.0000000' AS DateTime2), 0, CAST(N'2021-12-09T00:00:00.0000000' AS DateTime2), N'Izuku Midoriya', 6, 1, N'izuku', N'IZUKU', N'izuku@mail.com', N'IZUKU@MAIL.COM', 0, N'AQAAAAEAACcQAAAAENdItvaZmUVcqU4gQ+/OxFjQo3XcRgXO3ObB2nB2+OMv+EiqljZfk9QAEvoXzz9tbQ==', N'O5ZMWQ7QCI25BUVAI3DKEMPRU2VWMZPW', N'f113612d-701b-40b8-9a00-0cdf72d14fdd', N'+918725544775', 0, 0, NULL, 1, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0)
GO
INSERT [dbo].[AspNetUsers] ([Id], [DateOfBirth], [Gender], [LastActive], [LastName], [PhotoId], [CityId], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount], [FirstName], [JobCategoryId], [JobTypeId], [Biography], [Position], [CvFilePath], [CompanyId], [IsCompany]) VALUES (7, CAST(N'1981-05-14T00:00:00.0000000' AS DateTime2), 0, CAST(N'2021-02-02T00:00:00.0000000' AS DateTime2), N'Bailey Stephens', 7, 1, N'bailey', N'BAILEY', N'bailey@mail.com', N'BAILEY@MAIL.COM', 0, N'AQAAAAEAACcQAAAAEG9hGa0HHUZhY77aNd28xoXOjQElRFcO6+nArSrCYSoDvbva97yVcXPvP9wGUFKw3A==', N'DSOSAYNGXDEQHA55XW7QCCWMGC62KKLW', N'd6ed020e-80f0-452a-b4e5-aeca6af838f0', N'+918678019695', 0, 0, NULL, 1, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0)
GO
INSERT [dbo].[AspNetUsers] ([Id], [DateOfBirth], [Gender], [LastActive], [LastName], [PhotoId], [CityId], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount], [FirstName], [JobCategoryId], [JobTypeId], [Biography], [Position], [CvFilePath], [CompanyId], [IsCompany]) VALUES (8, CAST(N'1986-09-21T00:00:00.0000000' AS DateTime2), 0, CAST(N'2021-05-13T00:00:00.0000000' AS DateTime2), N'Summers Wong', 8, 1, N'summers', N'SUMMERS', N'summers@mail.com', N'SUMMERS@MAIL.COM', 0, N'AQAAAAEAACcQAAAAEHAo/VTPPmMRPmQfeyG7PG3F0qcVfIGJXTct0EYGIgFKPocwngpdFw7u3PKaye0csQ==', N'R527SAZSI7ZYYMHWKCBL3N24IRZ53LUH', N'649c5108-8a3e-4762-a5a9-da195b210dbd', N'+919496319034', 0, 0, NULL, 1, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0)
GO
INSERT [dbo].[AspNetUsers] ([Id], [DateOfBirth], [Gender], [LastActive], [LastName], [PhotoId], [CityId], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount], [FirstName], [JobCategoryId], [JobTypeId], [Biography], [Position], [CvFilePath], [CompanyId], [IsCompany]) VALUES (9, CAST(N'1989-05-22T00:00:00.0000000' AS DateTime2), 0, CAST(N'2021-05-21T00:00:00.0000000' AS DateTime2), N'Noble Figueroa', 9, 1, N'noble', N'NOBLE', N'noble@mail.com', N'NOBLE@MAIL.COM', 0, N'AQAAAAEAACcQAAAAEGxX1a63Zbw1Vdc3cjAjjHwbNoQOp0Kojc0+83U5Up34zicnXwQfdV/UF4zo9pOkGw==', N'UU4LY4FOIO2IOK3NYQBAGNTTUYVZUYK2', N'9d04f70a-ea88-4de6-a2c3-b6b6cf21d9d7', N'+918489132723', 0, 0, NULL, 1, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0)
GO
INSERT [dbo].[AspNetUsers] ([Id], [DateOfBirth], [Gender], [LastActive], [LastName], [PhotoId], [CityId], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount], [FirstName], [JobCategoryId], [JobTypeId], [Biography], [Position], [CvFilePath], [CompanyId], [IsCompany]) VALUES (10, CAST(N'1986-08-10T00:00:00.0000000' AS DateTime2), 0, CAST(N'2021-03-02T00:00:00.0000000' AS DateTime2), N'Reed Cortez', 10, 1, N'reed', N'REED', N'reed@mail.com', N'REED@MAIL.COM', 0, N'AQAAAAEAACcQAAAAEOYTjW7AV/LEqWO7JF2ZmC2Sgk8FyYO8fIn8bBJUacHFK+CA6QH2m9EdgpMHC8d5kA==', N'DUJI5DQPLFWSLER5DA6V7KDQHZ3CGHCA', N'b9d1aaaf-5487-46a9-98d1-ff80f1dca7f7', N'+919951780304', 0, 0, NULL, 1, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0)
GO
INSERT [dbo].[AspNetUsers] ([Id], [DateOfBirth], [Gender], [LastActive], [LastName], [PhotoId], [CityId], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount], [FirstName], [JobCategoryId], [JobTypeId], [Biography], [Position], [CvFilePath], [CompanyId], [IsCompany]) VALUES (11, CAST(N'1995-05-15T00:00:00.0000000' AS DateTime2), 0, CAST(N'2021-06-12T00:00:00.0000000' AS DateTime2), N'Thornton Roth', 11, 1, N'thornton', N'THORNTON', N'thornton@mail.com', N'THORNTON@MAIL.COM', 0, N'AQAAAAEAACcQAAAAEPkMg0dYNhCMXkXt2gFScE+8YCi0U3nShZtGBmG2EsilAWg1shzQOX7XILQ1599Iew==', N'LNDK3N7LRZEFMBGXK73KSSUJU7F7A2MG', N'477e00d5-8fb3-49ac-8306-e8425b70bb3b', N'+918462786675', 0, 0, NULL, 1, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0)
GO
INSERT [dbo].[AspNetUsers] ([Id], [DateOfBirth], [Gender], [LastActive], [LastName], [PhotoId], [CityId], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount], [FirstName], [JobCategoryId], [JobTypeId], [Biography], [Position], [CvFilePath], [CompanyId], [IsCompany]) VALUES (12, CAST(N'1989-04-07T00:00:00.0000000' AS DateTime2), 0, CAST(N'2021-09-03T00:00:00.0000000' AS DateTime2), N'Frazier Schneider', 12, 1, N'frazier', N'FRAZIER', N'frazier@mail.com', N'FRAZIER@MAIL.COM', 0, N'AQAAAAEAACcQAAAAEDkUfRm1+vzaYalAsspj6atyBavrDUjyHdvvroRjBnESEHzGtipYi698DUDMJS2fOw==', N'I6JHJFIESXGIUCNR3AGBTX2WVXJHHQOH', N'd9f6b8f0-1fd8-42a2-8ca0-4e326a3e72b9', N'+918144602417', 0, 0, NULL, 1, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0)
GO
INSERT [dbo].[AspNetUsers] ([Id], [DateOfBirth], [Gender], [LastActive], [LastName], [PhotoId], [CityId], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount], [FirstName], [JobCategoryId], [JobTypeId], [Biography], [Position], [CvFilePath], [CompanyId], [IsCompany]) VALUES (13, CAST(N'1988-08-26T00:00:00.0000000' AS DateTime2), 0, CAST(N'2021-07-01T00:00:00.0000000' AS DateTime2), N'Peterson Christian', 13, 1, N'peterson', N'PETERSON', N'peterson@mail.com', N'PETERSON@MAIL.COM', 0, N'AQAAAAEAACcQAAAAEDnGZV87ZmV52tcd+Ihe7Rinml3iOHq+e1KgAm11ZX0PoduS1E1AomBqBvqNEvlEXQ==', N'4KI7U3DT32ABXXTQHQYQCBO62VOFHMZ3', N'cec29df4-f737-4691-9ad4-4214fddfdddb', N'+919738910343', 0, 0, NULL, 1, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0)
GO
INSERT [dbo].[AspNetUsers] ([Id], [DateOfBirth], [Gender], [LastActive], [LastName], [PhotoId], [CityId], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount], [FirstName], [JobCategoryId], [JobTypeId], [Biography], [Position], [CvFilePath], [CompanyId], [IsCompany]) VALUES (14, CAST(N'1987-06-27T00:00:00.0000000' AS DateTime2), 0, CAST(N'2021-11-23T00:00:00.0000000' AS DateTime2), N'Powers Lee', 14, 1, N'powers', N'POWERS', N'powers@mail.com', N'POWERS@MAIL.COM', 0, N'AQAAAAEAACcQAAAAED11Qrwf74WOf8RaZvzKyb8KAMfeIuA9yM6QLnbyT305YkvesND8JigokiynQHdjxg==', N'CY273FGRKM523WVXBY2TC2DY6ZCEIWBC', N'ca6ea73b-e032-4d7a-9580-bb5e5c7ebf6f', N'+918773750335', 0, 0, NULL, 1, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0)
GO
INSERT [dbo].[AspNetUsers] ([Id], [DateOfBirth], [Gender], [LastActive], [LastName], [PhotoId], [CityId], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount], [FirstName], [JobCategoryId], [JobTypeId], [Biography], [Position], [CvFilePath], [CompanyId], [IsCompany]) VALUES (15, CAST(N'1992-08-18T00:00:00.0000000' AS DateTime2), 0, CAST(N'2021-06-05T00:00:00.0000000' AS DateTime2), N'Woods Peterson', 15, 1, N'woods', N'WOODS', N'woods@mail.com', N'WOODS@MAIL.COM', 0, N'AQAAAAEAACcQAAAAEG5P9+O2Fyq4LIzrtrepVklvaTdD21KKwueyWlJht3UMgafPMbjgHDN7Zrm+T+SY0w==', N'ETJOKYRSE6BHLXZTRE4AJ5GVDL2IRVXG', N'91f5fc79-ba37-45d9-a542-95dea51b9bb6', N'+919139179609', 0, 0, NULL, 1, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0)
GO
INSERT [dbo].[AspNetUsers] ([Id], [DateOfBirth], [Gender], [LastActive], [LastName], [PhotoId], [CityId], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount], [FirstName], [JobCategoryId], [JobTypeId], [Biography], [Position], [CvFilePath], [CompanyId], [IsCompany]) VALUES (16, CAST(N'1997-01-01T00:00:00.0000000' AS DateTime2), 0, CAST(N'2021-03-23T00:00:00.0000000' AS DateTime2), N'Sexton Hayden', 16, 1, N'sexton', N'SEXTON', N'sexton@mail.com', N'SEXTON@MAIL.COM', 0, N'AQAAAAEAACcQAAAAEDwzn1B3OXD3pRGDM07Ug6Z84+5bPwYjXyrtiamgh8Zoxwf2SUkmYMNUyDsB/5NzgQ==', N'SP7ZMAF7LWNX53XH3R2TEIJMI34MRBJ2', N'708b5fea-c10a-44c6-a985-bdf8e051d54c', N'+918817076207', 0, 0, NULL, 1, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0)
GO
INSERT [dbo].[AspNetUsers] ([Id], [DateOfBirth], [Gender], [LastActive], [LastName], [PhotoId], [CityId], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount], [FirstName], [JobCategoryId], [JobTypeId], [Biography], [Position], [CvFilePath], [CompanyId], [IsCompany]) VALUES (17, CAST(N'1982-01-22T00:00:00.0000000' AS DateTime2), 0, CAST(N'2021-03-15T00:00:00.0000000' AS DateTime2), N'Richard Gamble', 17, 1, N'richard', N'RICHARD', N'richard@mail.com', N'RICHARD@MAIL.COM', 0, N'AQAAAAEAACcQAAAAELlv928a0aEho//T3YZF+hJAeOa5d+aJLfUi/i+2KCGZFesI89pxbz9CQIv14lw+3Q==', N'K7K7IOG4AVAFAMTHZ6YEOPXCAYYR26YV', N'c50f0690-2b02-47d6-b58e-b9034778ce89', N'+918082357603', 0, 0, NULL, 1, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0)
GO
INSERT [dbo].[AspNetUsers] ([Id], [DateOfBirth], [Gender], [LastActive], [LastName], [PhotoId], [CityId], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount], [FirstName], [JobCategoryId], [JobTypeId], [Biography], [Position], [CvFilePath], [CompanyId], [IsCompany]) VALUES (18, CAST(N'1995-03-25T00:00:00.0000000' AS DateTime2), 0, CAST(N'2021-11-08T00:00:00.0000000' AS DateTime2), N'Bennett Tyson', 18, 1, N'bennett', N'BENNETT', N'bennett@mail.com', N'BENNETT@MAIL.COM', 0, N'AQAAAAEAACcQAAAAEEn4pRdHPBtCbwKEZ2HeNdpgb9Nz1aq8M7DGBkPpJJF7Blr1yIeYIxe9LktoLXwOFQ==', N'NHLMNNA6JRQOFGKH3VGZLEUSQ3THLH3E', N'070683bb-9b77-40b0-8c4f-7fdb4c9cf484', N'+918184482087', 0, 0, NULL, 1, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0)
GO
INSERT [dbo].[AspNetUsers] ([Id], [DateOfBirth], [Gender], [LastActive], [LastName], [PhotoId], [CityId], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount], [FirstName], [JobCategoryId], [JobTypeId], [Biography], [Position], [CvFilePath], [CompanyId], [IsCompany]) VALUES (19, CAST(N'1982-03-29T00:00:00.0000000' AS DateTime2), 0, CAST(N'2021-04-07T00:00:00.0000000' AS DateTime2), N'Burke Johnston', 19, 1, N'burke', N'BURKE', N'burke@mail.com', N'BURKE@MAIL.COM', 0, N'AQAAAAEAACcQAAAAELVMLjBtBjRTuJm3n2dDHYVoje4ip7egX/8iW3UCo5gs64aNvIWj5vz5tYWs/byODg==', N'P27NEF777CW76VCYE3MQT6M3BMHRMRWH', N'dc140987-e54e-4988-9474-ba796a24d6db', N'+918060906522', 0, 0, NULL, 1, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0)
GO
INSERT [dbo].[AspNetUsers] ([Id], [DateOfBirth], [Gender], [LastActive], [LastName], [PhotoId], [CityId], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount], [FirstName], [JobCategoryId], [JobTypeId], [Biography], [Position], [CvFilePath], [CompanyId], [IsCompany]) VALUES (20, CAST(N'1996-02-29T00:00:00.0000000' AS DateTime2), 0, CAST(N'2021-04-17T00:00:00.0000000' AS DateTime2), N'Padilla Padilla', 20, 1, N'padilla', N'PADILLA', N'padilla@mail.com', N'PADILLA@MAIL.COM', 0, N'AQAAAAEAACcQAAAAEAFr5XOS/SfFf9j2uoWdumCP35aT5HgvI6/yn3DiqbslsfAjdR797YBaH6qVvWBvjQ==', N'S4OLQ5IGRERP6KW55BZR7WAIMAUJI25V', N'317ec641-31a2-4517-9571-afaafe507048', N'+919651869917', 0, 0, NULL, 1, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0)
GO
INSERT [dbo].[AspNetUsers] ([Id], [DateOfBirth], [Gender], [LastActive], [LastName], [PhotoId], [CityId], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount], [FirstName], [JobCategoryId], [JobTypeId], [Biography], [Position], [CvFilePath], [CompanyId], [IsCompany]) VALUES (21, CAST(N'1980-12-24T00:00:00.0000000' AS DateTime2), 0, CAST(N'2021-02-19T00:00:00.0000000' AS DateTime2), N'Boyd Hyde', 21, 1, N'boyd', N'BOYD', N'boyd@mail.com', N'BOYD@MAIL.COM', 0, N'AQAAAAEAACcQAAAAEN4YbeJcxifq59PlHQRl/9Lv0HJNMmbnxtTjmMPXFQHZ7SSjNlgPmmB6ZB9+HXJ1eQ==', N'AALDW4UN3EROEWN5NGXWQQRIVCUCSHH4', N'a26845fa-c4c3-4c8a-a3d5-55c4343acc21', N'+919754185940', 0, 0, NULL, 1, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0)
GO
INSERT [dbo].[AspNetUsers] ([Id], [DateOfBirth], [Gender], [LastActive], [LastName], [PhotoId], [CityId], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount], [FirstName], [JobCategoryId], [JobTypeId], [Biography], [Position], [CvFilePath], [CompanyId], [IsCompany]) VALUES (22, CAST(N'1997-06-10T00:00:00.0000000' AS DateTime2), 0, CAST(N'2021-10-18T00:00:00.0000000' AS DateTime2), N'Osborn Burke', 22, 1, N'osborn', N'OSBORN', N'osborn@mail.com', N'OSBORN@MAIL.COM', 0, N'AQAAAAEAACcQAAAAEAjhi7hpbsY6+ZoN3MwY5bjCl0tdDk8fnjRE5vNwnAl4MDFNyzBdxe16NvpQynbP9g==', N'ZHRKQRHJ7Y6E77WXJH4XARKPO2DSBVQJ', N'24006394-647c-47d6-9f3e-5a3fbb374838', N'+918600002655', 0, 0, NULL, 1, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0)
GO
INSERT [dbo].[AspNetUsers] ([Id], [DateOfBirth], [Gender], [LastActive], [LastName], [PhotoId], [CityId], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount], [FirstName], [JobCategoryId], [JobTypeId], [Biography], [Position], [CvFilePath], [CompanyId], [IsCompany]) VALUES (23, CAST(N'1993-09-30T00:00:00.0000000' AS DateTime2), 0, CAST(N'2021-07-11T00:00:00.0000000' AS DateTime2), N'Hunter Marsh', 23, 1, N'hunter', N'HUNTER', N'hunter@mail.com', N'HUNTER@MAIL.COM', 0, N'AQAAAAEAACcQAAAAEJOwl0l+9hO1JDLXfoayLl0qFwXpVUyE6rirOZQkdysAj/2zSEaUn7/YtWzKdnb2EQ==', N'U3NXGXUYQNZENSNGP2CXEG4AH45RMHBG', N'e568adf5-8fb9-402a-b514-338ab2557848', N'+918820657746', 0, 0, NULL, 1, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0)
GO
INSERT [dbo].[AspNetUsers] ([Id], [DateOfBirth], [Gender], [LastActive], [LastName], [PhotoId], [CityId], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount], [FirstName], [JobCategoryId], [JobTypeId], [Biography], [Position], [CvFilePath], [CompanyId], [IsCompany]) VALUES (24, CAST(N'1984-05-13T00:00:00.0000000' AS DateTime2), 0, CAST(N'2021-08-31T00:00:00.0000000' AS DateTime2), N'Golden Dalton', 24, 1, N'golden', N'GOLDEN', N'golden@mail.com', N'GOLDEN@MAIL.COM', 0, N'AQAAAAEAACcQAAAAEOlC7bGt0Lll80tvbRZmGQlF07U7JpNBjj6fX1/NX1bTFvTKjc3IVIUQfnq0R+uMvA==', N'UDELY7ZNKHN7RORKROY453Y5FR4AOSLE', N'eefb99e1-fb5a-45e3-88df-8d74a25af5b2', N'+918459061921', 0, 0, NULL, 1, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0)
GO
INSERT [dbo].[AspNetUsers] ([Id], [DateOfBirth], [Gender], [LastActive], [LastName], [PhotoId], [CityId], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount], [FirstName], [JobCategoryId], [JobTypeId], [Biography], [Position], [CvFilePath], [CompanyId], [IsCompany]) VALUES (25, CAST(N'1988-02-14T00:00:00.0000000' AS DateTime2), 0, CAST(N'2021-08-22T00:00:00.0000000' AS DateTime2), N'Rogers Hensley', 25, 1, N'rogers', N'ROGERS', N'rogers@mail.com', N'ROGERS@MAIL.COM', 0, N'AQAAAAEAACcQAAAAEAXSlXynMqQ6rpcxtApxxJPuFH2Nke6zONBwFNA6jLZpMi9jfpt7wyF0xY5NT+VLOg==', N'RB5Y2SBJDMOTKUONCNFBAI67QG5V3JXM', N'5f9b06b5-f2ae-4403-8e6e-3c006a6477f4', N'+918809218916', 0, 0, NULL, 1, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0)
GO
INSERT [dbo].[AspNetUsers] ([Id], [DateOfBirth], [Gender], [LastActive], [LastName], [PhotoId], [CityId], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount], [FirstName], [JobCategoryId], [JobTypeId], [Biography], [Position], [CvFilePath], [CompanyId], [IsCompany]) VALUES (26, CAST(N'1999-08-15T00:00:00.0000000' AS DateTime2), 0, CAST(N'2021-12-27T00:00:00.0000000' AS DateTime2), N'Barlow Gilmore', 26, 1, N'barlow', N'BARLOW', N'barlow@mail.com', N'BARLOW@MAIL.COM', 0, N'AQAAAAEAACcQAAAAEMA72Lgd+2Z07bTVEinAdQOe/AKomp+UAX6PZ9WYp8AwcVVFhWPuvv4/e1g0psppNg==', N'WIEV7RIKGDW6MC5WH3MWBD6EM2M4G5G5', N'ad3fea71-7e31-4ca7-92c1-0ee75390fff2', N'+919860776195', 0, 0, NULL, 1, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0)
GO
INSERT [dbo].[AspNetUsers] ([Id], [DateOfBirth], [Gender], [LastActive], [LastName], [PhotoId], [CityId], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount], [FirstName], [JobCategoryId], [JobTypeId], [Biography], [Position], [CvFilePath], [CompanyId], [IsCompany]) VALUES (27, CAST(N'1998-01-19T00:00:00.0000000' AS DateTime2), 1, CAST(N'2021-06-30T00:00:00.0000000' AS DateTime2), N'Elsa Sloan', 27, 1, N'elsa', N'ELSA', N'elsa@mail.com', N'ELSA@MAIL.COM', 0, N'AQAAAAEAACcQAAAAEBKKXNkKN6UCkZQe817rmNa9uijJud353vzdXN/WFco66HYOYOHODDJrtfcm9aH/6Q==', N'Z55HNUXV7N6YIZ2DARNKJKPEFEOVI43X', N'1ca114d5-1ecf-4095-886d-2edc51ceefb8', N'+918551017447', 0, 0, NULL, 1, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0)
GO
INSERT [dbo].[AspNetUsers] ([Id], [DateOfBirth], [Gender], [LastActive], [LastName], [PhotoId], [CityId], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount], [FirstName], [JobCategoryId], [JobTypeId], [Biography], [Position], [CvFilePath], [CompanyId], [IsCompany]) VALUES (28, CAST(N'1991-01-09T00:00:00.0000000' AS DateTime2), 1, CAST(N'2021-03-17T00:00:00.0000000' AS DateTime2), N'Rem Mays', 28, 1, N'rem', N'REM', N'rem@mail.com', N'REM@MAIL.COM', 0, N'AQAAAAEAACcQAAAAEKahR/UQ4enaWZlg2ab+C6DSQb4GbRjphmqZydfqK6PXeFjjyoI6ZegRHzZ6K8FRZQ==', N'PIE6DYXV5DREZYYJEJ23RFR7YRCZMB3E', N'4ff4f0cf-edb0-4765-9856-cac24dbdcc13', N'+919019335726', 0, 0, NULL, 1, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0)
GO
INSERT [dbo].[AspNetUsers] ([Id], [DateOfBirth], [Gender], [LastActive], [LastName], [PhotoId], [CityId], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount], [FirstName], [JobCategoryId], [JobTypeId], [Biography], [Position], [CvFilePath], [CompanyId], [IsCompany]) VALUES (29, CAST(N'1987-05-24T00:00:00.0000000' AS DateTime2), 1, CAST(N'2021-09-06T00:00:00.0000000' AS DateTime2), N'Asuna Yuuki', 29, 1, N'asuna', N'ASUNA', N'asuna@mail.com', N'ASUNA@MAIL.COM', 0, N'AQAAAAEAACcQAAAAEIvOmxl0vxP0cJ4ZrB2cgBsDaCq5DQLApfy4jFtQfo7RSzExqDO6Rysw3idbkE/DoA==', N'YBIWZ5NVHGSRXNZLOCAH3DJZBH4X7FPL', N'dba22f7f-d4f5-499e-893a-bf2f953332ab', N'+918962553108', 0, 0, NULL, 1, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0)
GO
INSERT [dbo].[AspNetUsers] ([Id], [DateOfBirth], [Gender], [LastActive], [LastName], [PhotoId], [CityId], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount], [FirstName], [JobCategoryId], [JobTypeId], [Biography], [Position], [CvFilePath], [CompanyId], [IsCompany]) VALUES (30, CAST(N'1980-07-10T00:00:00.0000000' AS DateTime2), 1, CAST(N'2021-02-17T00:00:00.0000000' AS DateTime2), N'Miku Nakano', 30, 1, N'miku', N'MIKU', N'miku@mail.com', N'MIKU@MAIL.COM', 0, N'AQAAAAEAACcQAAAAEAQe2AhX++uI2jm5O7I8b9/XmT8fURbCilJsFkJln6kUMWHM230KRQhmdV4iYcgF9A==', N'DFNX7HOQTUKQKEZMLE4AVDGKLZZ2D2DH', N'1a8d185f-10b0-4b5b-a172-9ada16832fdf', N'+918924523382', 0, 0, NULL, 1, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0)
GO
INSERT [dbo].[AspNetUsers] ([Id], [DateOfBirth], [Gender], [LastActive], [LastName], [PhotoId], [CityId], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount], [FirstName], [JobCategoryId], [JobTypeId], [Biography], [Position], [CvFilePath], [CompanyId], [IsCompany]) VALUES (31, CAST(N'1994-07-18T00:00:00.0000000' AS DateTime2), 1, CAST(N'2021-04-25T00:00:00.0000000' AS DateTime2), N'Opal Cabrera', 31, 1, N'opal', N'OPAL', N'opal@mail.com', N'OPAL@MAIL.COM', 0, N'AQAAAAEAACcQAAAAEHoiDr7eBnjqIaJ1Zkv8+PN0vcB6DtO/3G/ZeXN4mvMKwBaXxOgxcf+i9ks52UlL0g==', N'EXXRTRKKNF2VFKL2CSD5BUXN7QGZWUSE', N'ed4d5a10-c00a-455c-9fa3-52f596889fce', N'+919154491678', 0, 0, NULL, 1, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0)
GO
INSERT [dbo].[AspNetUsers] ([Id], [DateOfBirth], [Gender], [LastActive], [LastName], [PhotoId], [CityId], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount], [FirstName], [JobCategoryId], [JobTypeId], [Biography], [Position], [CvFilePath], [CompanyId], [IsCompany]) VALUES (32, CAST(N'1981-08-18T00:00:00.0000000' AS DateTime2), 1, CAST(N'2021-11-21T00:00:00.0000000' AS DateTime2), N'Cristina Acevedo', 32, 1, N'cristina', N'CRISTINA', N'cristina@mail.com', N'CRISTINA@MAIL.COM', 0, N'AQAAAAEAACcQAAAAELQmUw3d1frBzJmy5+QYkLCv+KYPUbmZQFRPa7eYGfF6xGTD4EgQ4DfIr3aS82eEdQ==', N'FETXS76KUEDH3DO25L3O3BFJ3IX2WBAI', N'5526d73b-5f3f-43cf-95ad-0ee2b8cb1360', N'+918928528079', 0, 0, NULL, 1, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0)
GO
INSERT [dbo].[AspNetUsers] ([Id], [DateOfBirth], [Gender], [LastActive], [LastName], [PhotoId], [CityId], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount], [FirstName], [JobCategoryId], [JobTypeId], [Biography], [Position], [CvFilePath], [CompanyId], [IsCompany]) VALUES (33, CAST(N'1990-04-29T00:00:00.0000000' AS DateTime2), 1, CAST(N'2021-02-19T00:00:00.0000000' AS DateTime2), N'Allison Holman', 33, 1, N'allison', N'ALLISON', N'allison@mail.com', N'ALLISON@MAIL.COM', 0, N'AQAAAAEAACcQAAAAEBoU8bDnlaDC8smHH8HUD/nQGjn7uSjS0BlZX8k3TpKVbLom2XYik4lTNwkkDL9UGw==', N'PHNQ4AGV74XEU5EGZDXMW674GMJUXBJH', N'47740063-44b8-43b2-a162-c03fcfd5a1a8', N'+918963166045', 0, 0, NULL, 1, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0)
GO
INSERT [dbo].[AspNetUsers] ([Id], [DateOfBirth], [Gender], [LastActive], [LastName], [PhotoId], [CityId], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount], [FirstName], [JobCategoryId], [JobTypeId], [Biography], [Position], [CvFilePath], [CompanyId], [IsCompany]) VALUES (34, CAST(N'1997-02-16T00:00:00.0000000' AS DateTime2), 1, CAST(N'2021-12-08T00:00:00.0000000' AS DateTime2), N'Katie Meadows', 34, 1, N'katie', N'KATIE', N'katie@mail.com', N'KATIE@MAIL.COM', 0, N'AQAAAAEAACcQAAAAEJ9DAXGjdTllbPohmEccehA1SoqQzxWvQdRq8hg+EWOdisLN4jvnJVOWNuBdLVOEhw==', N'ANF6RDM43BDWWMKD36RXVPTW3LBQEYO7', N'14f4e3b8-f5a7-41c2-9f57-47b27b767de3', N'+918158130364', 0, 0, NULL, 1, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0)
GO
INSERT [dbo].[AspNetUsers] ([Id], [DateOfBirth], [Gender], [LastActive], [LastName], [PhotoId], [CityId], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount], [FirstName], [JobCategoryId], [JobTypeId], [Biography], [Position], [CvFilePath], [CompanyId], [IsCompany]) VALUES (35, CAST(N'1985-03-18T00:00:00.0000000' AS DateTime2), 1, CAST(N'2021-03-10T00:00:00.0000000' AS DateTime2), N'Dionne Rutledge', 35, 1, N'dionne', N'DIONNE', N'dionne@mail.com', N'DIONNE@MAIL.COM', 0, N'AQAAAAEAACcQAAAAEF6Zk6ALfXKtDIc8LSNMRYSWuEsIass5IM5iHH7Qv+iZnaK2OG7uQ2tV+JJcAQQ3uA==', N'2KHQHYW2CKRVYQRCNLTKLW5SYBHD4YQF', N'70e1b81e-0bef-4f81-b510-156aa022a19f', N'+919107086100', 0, 0, NULL, 1, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0)
GO
INSERT [dbo].[AspNetUsers] ([Id], [DateOfBirth], [Gender], [LastActive], [LastName], [PhotoId], [CityId], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount], [FirstName], [JobCategoryId], [JobTypeId], [Biography], [Position], [CvFilePath], [CompanyId], [IsCompany]) VALUES (36, CAST(N'1980-09-10T00:00:00.0000000' AS DateTime2), 1, CAST(N'2021-11-02T00:00:00.0000000' AS DateTime2), N'Carla Garcia', 36, 1, N'carla', N'CARLA', N'carla@mail.com', N'CARLA@MAIL.COM', 0, N'AQAAAAEAACcQAAAAEFiwOxyH6K5XXAz9ERR1w4THX+DZrdBAb1BqKSEw/DbqI3Ly3Uod7o9bWLxecZ2NzA==', N'YRNGVMGXSPJPN62AHD7EVJ7NO5WC5EIL', N'f9788fb2-284b-4050-bc88-69c04c27910f', N'+918669458598', 0, 0, NULL, 1, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0)
GO
INSERT [dbo].[AspNetUsers] ([Id], [DateOfBirth], [Gender], [LastActive], [LastName], [PhotoId], [CityId], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount], [FirstName], [JobCategoryId], [JobTypeId], [Biography], [Position], [CvFilePath], [CompanyId], [IsCompany]) VALUES (37, CAST(N'1990-11-03T00:00:00.0000000' AS DateTime2), 1, CAST(N'2021-03-02T00:00:00.0000000' AS DateTime2), N'Rae Gordon', 37, 1, N'rae', N'RAE', N'rae@mail.com', N'RAE@MAIL.COM', 0, N'AQAAAAEAACcQAAAAECrdqJ8IEkb3Fp9Z8DpJMuMleZga4QY1dznmCtgmALwqZ9doaYYapaUOrxVHRSsLug==', N'OXYDADX7NLLQY6WAX7UXJAL533WIX3WI', N'f740577e-c20c-40d5-969c-3a5f11da0ff6', N'+919298663862', 0, 0, NULL, 1, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0)
GO
INSERT [dbo].[AspNetUsers] ([Id], [DateOfBirth], [Gender], [LastActive], [LastName], [PhotoId], [CityId], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount], [FirstName], [JobCategoryId], [JobTypeId], [Biography], [Position], [CvFilePath], [CompanyId], [IsCompany]) VALUES (38, CAST(N'1997-08-28T00:00:00.0000000' AS DateTime2), 1, CAST(N'2021-02-12T00:00:00.0000000' AS DateTime2), N'Jamie Morris', 38, 1, N'jamie', N'JAMIE', N'jamie@mail.com', N'JAMIE@MAIL.COM', 0, N'AQAAAAEAACcQAAAAEHvfDbDlT2LHtvTkfXLwuidsNIzzKV6UVyRS4p8fFgFtOyyERfzhAujSkdC3E0/l4A==', N'LQU3XVZ3QFL7PBJ5JTYOVY4TCWFVKVP6', N'fd98ff0b-d29b-4df5-aa46-9c2d8e06e3a1', N'+919215488364', 0, 0, NULL, 1, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0)
GO
INSERT [dbo].[AspNetUsers] ([Id], [DateOfBirth], [Gender], [LastActive], [LastName], [PhotoId], [CityId], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount], [FirstName], [JobCategoryId], [JobTypeId], [Biography], [Position], [CvFilePath], [CompanyId], [IsCompany]) VALUES (39, CAST(N'1990-03-20T00:00:00.0000000' AS DateTime2), 1, CAST(N'2021-07-03T00:00:00.0000000' AS DateTime2), N'Ebony Burnett', 39, 1, N'ebony', N'EBONY', N'ebony@mail.com', N'EBONY@MAIL.COM', 0, N'AQAAAAEAACcQAAAAEAEfuc5U0lfSdHllwe7wNuzmVbIEaTC7Est00XiUhdZ1KFeIDnWfDL2MNaPgBXkB0g==', N'KZEDORNOSIF435UXTIIPEYLSQGQOIZ3R', N'8b018179-dc7e-4ec3-926c-4288dbc6c9d8', N'+919668495474', 0, 0, NULL, 1, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0)
GO
INSERT [dbo].[AspNetUsers] ([Id], [DateOfBirth], [Gender], [LastActive], [LastName], [PhotoId], [CityId], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount], [FirstName], [JobCategoryId], [JobTypeId], [Biography], [Position], [CvFilePath], [CompanyId], [IsCompany]) VALUES (40, CAST(N'1985-07-13T00:00:00.0000000' AS DateTime2), 1, CAST(N'2021-07-07T00:00:00.0000000' AS DateTime2), N'Frieda Hodge', 40, 1, N'frieda', N'FRIEDA', N'frieda@mail.com', N'FRIEDA@MAIL.COM', 0, N'AQAAAAEAACcQAAAAEHA7Spyv9frsABRF1VVsVd2DPrkcibI4uX80b8zl4mEuHogbJAdK/OXYN38NXoCowQ==', N'HCNWRJNXTBJZU5QMOHZKOV7DW2CIEUF6', N'2f87901a-659a-4e28-8929-248437d2730a', N'+919807093198', 0, 0, NULL, 1, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0)
GO
INSERT [dbo].[AspNetUsers] ([Id], [DateOfBirth], [Gender], [LastActive], [LastName], [PhotoId], [CityId], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount], [FirstName], [JobCategoryId], [JobTypeId], [Biography], [Position], [CvFilePath], [CompanyId], [IsCompany]) VALUES (41, CAST(N'1982-12-04T00:00:00.0000000' AS DateTime2), 1, CAST(N'2021-07-09T00:00:00.0000000' AS DateTime2), N'Elsie Bates', 41, 1, N'elsie', N'ELSIE', N'elsie@mail.com', N'ELSIE@MAIL.COM', 0, N'AQAAAAEAACcQAAAAELk3Sk8u0HAEzg9p5iWfaVhh6a3f1t7XtWsyzDPxR8rGfVPw3nlAdeon5zbiKtjbBg==', N'42QQBFFL47IF3LJ3N2JUJSMTTLQAQZ6L', N'3cea7e61-6d9a-4151-8bbc-40e6740fb868', N'+918227189770', 0, 0, NULL, 1, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0)
GO
INSERT [dbo].[AspNetUsers] ([Id], [DateOfBirth], [Gender], [LastActive], [LastName], [PhotoId], [CityId], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount], [FirstName], [JobCategoryId], [JobTypeId], [Biography], [Position], [CvFilePath], [CompanyId], [IsCompany]) VALUES (42, CAST(N'1980-08-20T00:00:00.0000000' AS DateTime2), 1, CAST(N'2021-08-03T00:00:00.0000000' AS DateTime2), N'Traci Hampton', 42, 1, N'traci', N'TRACI', N'traci@mail.com', N'TRACI@MAIL.COM', 0, N'AQAAAAEAACcQAAAAEL9kjo/ITDSRAMS/2Hkrlh1mwjTOMtA8/IjG2Y7irWuQueudHGatKhw1cBc+wMwHmQ==', N'CJJWRASG3VEMT56KIZLZY7ZHNLMDPKPL', N'4556ebd1-432f-4f59-92e3-fe2c382ce09c', N'+919510503172', 0, 0, NULL, 1, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0)
GO
INSERT [dbo].[AspNetUsers] ([Id], [DateOfBirth], [Gender], [LastActive], [LastName], [PhotoId], [CityId], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount], [FirstName], [JobCategoryId], [JobTypeId], [Biography], [Position], [CvFilePath], [CompanyId], [IsCompany]) VALUES (43, CAST(N'1991-06-15T00:00:00.0000000' AS DateTime2), 1, CAST(N'2021-11-15T00:00:00.0000000' AS DateTime2), N'Jannie Hahn', 43, 1, N'jannie', N'JANNIE', N'jannie@mail.com', N'JANNIE@MAIL.COM', 0, N'AQAAAAEAACcQAAAAEPL542GrxtyxHQhvELBPG05ZjGkKuttz8WbHR1mMPXbztDhtAdJ7WnctRSs5wXrtWw==', N'OCRDNYS2FGDU2ZIEJGN2ZVASDXEQV6QH', N'c348c512-3b81-4eba-b4a2-81e4f8d13f9a', N'+919535081193', 0, 0, NULL, 1, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0)
GO
INSERT [dbo].[AspNetUsers] ([Id], [DateOfBirth], [Gender], [LastActive], [LastName], [PhotoId], [CityId], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount], [FirstName], [JobCategoryId], [JobTypeId], [Biography], [Position], [CvFilePath], [CompanyId], [IsCompany]) VALUES (44, CAST(N'1991-04-12T00:00:00.0000000' AS DateTime2), 1, CAST(N'2021-07-02T00:00:00.0000000' AS DateTime2), N'Victoria Moran', 44, 1, N'victoria', N'VICTORIA', N'victoria@mail.com', N'VICTORIA@MAIL.COM', 0, N'AQAAAAEAACcQAAAAEAWwlV7sJvCylhkxwIki5J2II9lD6bbOcQJlwH5aUXpg0fDapzpOiTjjoz2ftZUMeA==', N'M2G6UPAWXM5GF2OYEJZDVLBEMHUFIU3N', N'785c225e-90ed-45a4-95e4-464194798eb0', N'+918079222893', 0, 0, NULL, 1, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0)
GO
INSERT [dbo].[AspNetUsers] ([Id], [DateOfBirth], [Gender], [LastActive], [LastName], [PhotoId], [CityId], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount], [FirstName], [JobCategoryId], [JobTypeId], [Biography], [Position], [CvFilePath], [CompanyId], [IsCompany]) VALUES (45, CAST(N'1998-11-30T00:00:00.0000000' AS DateTime2), 1, CAST(N'2021-12-24T00:00:00.0000000' AS DateTime2), N'Gwen Oconnor', 45, 1, N'gwen', N'GWEN', N'gwen@mail.com', N'GWEN@MAIL.COM', 0, N'AQAAAAEAACcQAAAAEEDVjk3C05G2Glay/5yULWNXLVWuy5KNb5dLcXzl9P5gto9yUM9DbU2znwdMczFy+w==', N'VD776766BFX7BKNJOFFBZL4LFCVTUOG7', N'30338fac-e5c6-4269-84cc-a26ae8c866e8', N'+918524987373', 0, 0, NULL, 1, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0)
GO
INSERT [dbo].[AspNetUsers] ([Id], [DateOfBirth], [Gender], [LastActive], [LastName], [PhotoId], [CityId], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount], [FirstName], [JobCategoryId], [JobTypeId], [Biography], [Position], [CvFilePath], [CompanyId], [IsCompany]) VALUES (46, CAST(N'1997-03-31T00:00:00.0000000' AS DateTime2), 1, CAST(N'2021-12-01T00:00:00.0000000' AS DateTime2), N'Kim James', 46, 1, N'kim', N'KIM', N'kim@mail.com', N'KIM@MAIL.COM', 0, N'AQAAAAEAACcQAAAAECQqsfciAsiyreXL/d8Lsy/4OTYZg7Fxk48qRDXbeIZEbPbvkV6SeM2RVd0lCsEKyg==', N'QS2CMJUPVOLMC37WM7HXZLENOQYFC4AY', N'c8235937-4fb5-4aec-ac28-5dda648a6e4c', N'+919647750168', 0, 0, NULL, 1, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0)
GO
INSERT [dbo].[AspNetUsers] ([Id], [DateOfBirth], [Gender], [LastActive], [LastName], [PhotoId], [CityId], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount], [FirstName], [JobCategoryId], [JobTypeId], [Biography], [Position], [CvFilePath], [CompanyId], [IsCompany]) VALUES (47, CAST(N'1983-07-05T00:00:00.0000000' AS DateTime2), 1, CAST(N'2021-09-03T00:00:00.0000000' AS DateTime2), N'Shawna Manning', 47, 1, N'shawna', N'SHAWNA', N'shawna@mail.com', N'SHAWNA@MAIL.COM', 0, N'AQAAAAEAACcQAAAAEGFVVa8it+2AmomoD00RgieTe2+N040xn1oBA0p/bEix9xgJl20vPnF4nx+LICEkjQ==', N'W6YHNDQQB64PWFCWB22XDSP4MRKV36HB', N'6da75477-669c-4d0b-a115-e51953776fd1', N'+919209081439', 0, 0, NULL, 1, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0)
GO
INSERT [dbo].[AspNetUsers] ([Id], [DateOfBirth], [Gender], [LastActive], [LastName], [PhotoId], [CityId], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount], [FirstName], [JobCategoryId], [JobTypeId], [Biography], [Position], [CvFilePath], [CompanyId], [IsCompany]) VALUES (48, CAST(N'1983-10-27T00:00:00.0000000' AS DateTime2), 1, CAST(N'2021-02-13T00:00:00.0000000' AS DateTime2), N'Latoya Young', 48, 1, N'latoya', N'LATOYA', N'latoya@mail.com', N'LATOYA@MAIL.COM', 0, N'AQAAAAEAACcQAAAAEOVTjKUpryPdcpXHaEtMwo3opmvSHHfvgOetr5TvUdMCPCnT4EAIriwf6PI6PQCaOQ==', N'XGGS4RPMK2EKMNF5B2M7A6BLE7JDXFY6', N'2f41a5d3-d89d-4ba2-9f92-685e1872ddf1', N'+918068763831', 0, 0, NULL, 1, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0)
GO
INSERT [dbo].[AspNetUsers] ([Id], [DateOfBirth], [Gender], [LastActive], [LastName], [PhotoId], [CityId], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount], [FirstName], [JobCategoryId], [JobTypeId], [Biography], [Position], [CvFilePath], [CompanyId], [IsCompany]) VALUES (49, CAST(N'1986-01-10T00:00:00.0000000' AS DateTime2), 1, CAST(N'2021-05-04T00:00:00.0000000' AS DateTime2), N'Josie Merrill', 49, 1, N'josie', N'JOSIE', N'josie@mail.com', N'JOSIE@MAIL.COM', 0, N'AQAAAAEAACcQAAAAECQqVvOXqcuQHblMAItB4CWv2mcKu1URczHHDhEBMlG8YVQ0lkecaFGyKgq2aiDS5w==', N'PO3CNFN34ST3BDPO7AAR64IJJAQDZY6I', N'6fd6e5c8-8dc7-403c-92f0-98c89e8dc839', N'+918973496318', 0, 0, NULL, 1, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0)
GO
INSERT [dbo].[AspNetUsers] ([Id], [DateOfBirth], [Gender], [LastActive], [LastName], [PhotoId], [CityId], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount], [FirstName], [JobCategoryId], [JobTypeId], [Biography], [Position], [CvFilePath], [CompanyId], [IsCompany]) VALUES (50, CAST(N'1991-10-08T00:00:00.0000000' AS DateTime2), 1, CAST(N'2021-03-02T00:00:00.0000000' AS DateTime2), N'Casandra Crawford', 50, 1, N'casandra', N'CASANDRA', N'casandra@mail.com', N'CASANDRA@MAIL.COM', 0, N'AQAAAAEAACcQAAAAEDN3w8Nd3tA9hQ19ZFaaJr+bke6WcxkjWz8cM9tHOXuR2pc104nw0yBx486ws9duZA==', N'3ZIP6D6WOQYSFQOSQCXFKM673TWJHPC5', N'5521afd5-1257-4706-a567-4f81c490185e', N'+918104900032', 0, 0, NULL, 1, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0)
GO
INSERT [dbo].[AspNetUsers] ([Id], [DateOfBirth], [Gender], [LastActive], [LastName], [PhotoId], [CityId], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount], [FirstName], [JobCategoryId], [JobTypeId], [Biography], [Position], [CvFilePath], [CompanyId], [IsCompany]) VALUES (51, CAST(N'1988-06-21T00:00:00.0000000' AS DateTime2), 1, CAST(N'2021-11-11T00:00:00.0000000' AS DateTime2), N'Judy Norman', 51, 1, N'judy', N'JUDY', N'judy@mail.com', N'JUDY@MAIL.COM', 0, N'AQAAAAEAACcQAAAAECDv09xbJxUK77NQIOP089NLjr5K8L3IymifOInmuMb5LcasKYYnNleDXXxH46aG0Q==', N'QDCZTKYCLR3BV3ASETC7GSKNYVPNLVC2', N'56c20b85-c6fa-44c5-ae1d-b0f1336786ce', N'+918881439966', 0, 0, NULL, 1, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0)
GO
INSERT [dbo].[AspNetUsers] ([Id], [DateOfBirth], [Gender], [LastActive], [LastName], [PhotoId], [CityId], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount], [FirstName], [JobCategoryId], [JobTypeId], [Biography], [Position], [CvFilePath], [CompanyId], [IsCompany]) VALUES (52, CAST(N'1997-07-23T20:00:00.0000000' AS DateTime2), 0, CAST(N'2024-03-24T12:49:30.1776037' AS DateTime2), N'emin dukic', 52, 1, N'emin12312', N'EMIN12312', N'emindukic123@gmail.com', N'EMINDUKIC123@GMAIL.COM', 0, N'AQAAAAEAACcQAAAAEDABhkvAOSXl+BgPBIbYKC9KXBH4/bMFbfu8Nl3GD3fvDK9F2OchuoBFwpPiwylryA==', N'LZQOW6NI3MPHADW5S2H4ELE7PAUHIPSV', N'60357e0f-dad9-4b2c-ad1f-ed888bd5a17d', N'+38762917495', 0, 0, NULL, 1, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0)
GO
INSERT [dbo].[AspNetUsers] ([Id], [DateOfBirth], [Gender], [LastActive], [LastName], [PhotoId], [CityId], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount], [FirstName], [JobCategoryId], [JobTypeId], [Biography], [Position], [CvFilePath], [CompanyId], [IsCompany]) VALUES (53, CAST(N'2024-02-29T23:00:00.0000000' AS DateTime2), 0, CAST(N'2024-03-24T18:17:48.8587914' AS DateTime2), NULL, 53, 1, N'tarik1', N'TARIK1', N'tarik@gmail.com', N'TARIK@GMAIL.COM', 0, N'AQAAAAEAACcQAAAAEOhSA3ntHU8yQVuetjm0RHV8qDe13MIt9idq/4SPwRFQsgmWPVN/8lI54iGiBRur+Q==', N'OFDG5S6FQXYWGY6GA33JBU4HXDRKYBHF', N'8b6b6d74-3e97-46d3-a295-e55503524697', N'1', 0, 0, NULL, 1, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0)
GO
INSERT [dbo].[AspNetUsers] ([Id], [DateOfBirth], [Gender], [LastActive], [LastName], [PhotoId], [CityId], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount], [FirstName], [JobCategoryId], [JobTypeId], [Biography], [Position], [CvFilePath], [CompanyId], [IsCompany]) VALUES (54, CAST(N'2024-02-29T23:00:00.0000000' AS DateTime2), 1, CAST(N'2024-03-24T18:26:50.8331076' AS DateTime2), N'dukic', 54, 1, N'behijad', N'BEHIJAD', N'behija@gmail.com', N'BEHIJA@GMAIL.COM', 0, N'AQAAAAEAACcQAAAAEJUeR8tpJkB2CCcCMz0N3o4AqGgjPUpCwLM1RdriObx2qHQXxyuiopJcclkrJZyVbw==', N'HUTCGYBCN45ZRKQHZJ3NOSYYZXGHRKDQ', N'a812be9d-b3a7-4a6d-8cb8-54297d40c196', N'1', 0, 0, NULL, 1, 0, N'behija', NULL, NULL, NULL, NULL, NULL, NULL, 0)
GO
INSERT [dbo].[AspNetUsers] ([Id], [DateOfBirth], [Gender], [LastActive], [LastName], [PhotoId], [CityId], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount], [FirstName], [JobCategoryId], [JobTypeId], [Biography], [Position], [CvFilePath], [CompanyId], [IsCompany]) VALUES (55, CAST(N'1995-05-19T08:00:00.0000000' AS DateTime2), 0, CAST(N'2024-03-24T22:26:26.7727556' AS DateTime2), N'dukic', 55, 2, N'harisd', N'HARISD', N'haris@gmail.com', N'HARIS@GMAIL.COM', 0, N'AQAAAAEAACcQAAAAEN+Fn62rACosCh47b1ry/385/sQeFK4y3uNbV6RW6AI8N7ZNt0IPrA+NK+OhJLD5ww==', N'GKZ7464Q7DRHAMNERWEIZ5J3RZBOV7CK', N'b552ad83-0864-440f-a160-825c5e890754', N'1', 0, 0, NULL, 1, 0, N'haris', 1, 3, NULL, NULL, NULL, NULL, 0)
GO
INSERT [dbo].[AspNetUsers] ([Id], [DateOfBirth], [Gender], [LastActive], [LastName], [PhotoId], [CityId], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount], [FirstName], [JobCategoryId], [JobTypeId], [Biography], [Position], [CvFilePath], [CompanyId], [IsCompany]) VALUES (56, CAST(N'1997-07-23T16:00:00.0000000' AS DateTime2), 0, CAST(N'2024-06-30T14:12:24.1162463' AS DateTime2), N'dukic', 1052, 1, N'test@test.com', N'TEST@TEST.COM', N'test@test.com', N'TEST@TEST.COM', 0, N'AQAAAAEAACcQAAAAEIaQjgdMmNbxZPZd1fKZgOQ+x5TCIhj82/xVauuugJQ1APwfDFxVJ/8toIqSpJ96Dw==', N'MSZW72K6KJRPCDX6ESAU264VDTWTHTR3', N'87066c51-01e4-42e2-bf4d-656dbe1e6412', N'+38762917495', 0, 0, NULL, 1, 0, N'emin', 4, 2, N'Full stack inzinjer', N'Inzinjer', N'C:\dev\web-shop\web-shop\API\uploads\EminDukic_CV.pdf', NULL, 0)
GO
INSERT [dbo].[AspNetUsers] ([Id], [DateOfBirth], [Gender], [LastActive], [LastName], [PhotoId], [CityId], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount], [FirstName], [JobCategoryId], [JobTypeId], [Biography], [Position], [CvFilePath], [CompanyId], [IsCompany]) VALUES (58, CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), 0, CAST(N'2024-07-14T16:27:26.6134057' AS DateTime2), NULL, 1054, 2, N'test@tes.com', N'TEST@TES.COM', N'test@tes.com', N'TEST@TES.COM', 0, N'AQAAAAEAACcQAAAAEOg1eBZFWw5lLgyaFD+KhsZiw1lXQenWSXjxXj6jL/gFSRYE7UnLXSe/cImtCSWU9w==', N'YGG4JGHCD7TAK5MMAMG6MLGAVXYWY5SN', N'bcfe7990-814a-42e0-9116-83286d1b3704', N'223', 0, 0, NULL, 1, 0, NULL, NULL, NULL, NULL, NULL, NULL, 2, 1)
GO
INSERT [dbo].[AspNetUsers] ([Id], [DateOfBirth], [Gender], [LastActive], [LastName], [PhotoId], [CityId], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount], [FirstName], [JobCategoryId], [JobTypeId], [Biography], [Position], [CvFilePath], [CompanyId], [IsCompany]) VALUES (59, CAST(N'2024-06-30T22:00:00.0000000' AS DateTime2), 0, CAST(N'2024-07-14T17:00:40.5708862' AS DateTime2), N'aa', 1055, 1, N'aa@1.com', N'AA@1.COM', N'aa@1.com', N'AA@1.COM', 0, N'AQAAAAEAACcQAAAAEE+/FFjk4aq0eZgJL6kSUGlBpxOzPrEu59WLKR6kG8yg4mT5MFy+CSNSZWN1CV9HsA==', N'Q3F2DDXSXLEMKK36LSQGCXC2MELV3OS2', N'e83bf999-fdd3-420a-9f81-e5f8595242eb', N'22', 0, 0, NULL, 1, 0, N'aa', NULL, NULL, NULL, NULL, NULL, NULL, 0)
GO
SET IDENTITY_INSERT [dbo].[AspNetUsers] OFF
GO
SET IDENTITY_INSERT [dbo].[Cities] ON 
GO
INSERT [dbo].[Cities] ([Id], [Name], [PostalCode], [CountryId]) VALUES (1, N'Sarajevo', N'71000', 1)
GO
INSERT [dbo].[Cities] ([Id], [Name], [PostalCode], [CountryId]) VALUES (2, N'Zenica', N'72000', 1)
GO
INSERT [dbo].[Cities] ([Id], [Name], [PostalCode], [CountryId]) VALUES (3, N'Tuzla', N'73000', 1)
GO
SET IDENTITY_INSERT [dbo].[Cities] OFF
GO
SET IDENTITY_INSERT [dbo].[Companies] ON 
GO
INSERT [dbo].[Companies] ([Id], [CompanyName], [CityId], [Address], [Email], [PhoneNumber], [AboutUs]) VALUES (1, N'test', 1, N'test', N'test12@test.com', N'22', NULL)
GO
INSERT [dbo].[Companies] ([Id], [CompanyName], [CityId], [Address], [Email], [PhoneNumber], [AboutUs]) VALUES (2, N'test22', 2, N'tereRERE', N'test@tes.com', N'223', N'AAAARR')
GO
SET IDENTITY_INSERT [dbo].[Companies] OFF
GO
SET IDENTITY_INSERT [dbo].[CompanyJobPosts] ON 
GO
INSERT [dbo].[CompanyJobPosts] ([Id], [JobDescription], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [CityId], [AdStartDate], [AdEndDate], [AdDuration], [IsDeleted], [AdName], [Position], [EmailForReceivingApplications]) VALUES (1, N'Test', CAST(N'2024-07-15T22:17:54.8500000' AS DateTime2), CAST(N'2024-07-15T22:17:54.8500000' AS DateTime2), 58, 1, 2, 1, 1, CAST(N'2024-07-15T20:17:24.8060000' AS DateTime2), CAST(N'2024-07-30T20:17:24.8060000' AS DateTime2), 15, 0, NULL, NULL, NULL)
GO
INSERT [dbo].[CompanyJobPosts] ([Id], [JobDescription], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [CityId], [AdStartDate], [AdEndDate], [AdDuration], [IsDeleted], [AdName], [Position], [EmailForReceivingApplications]) VALUES (2, N'Test', CAST(N'2024-07-15T22:54:14.4766667' AS DateTime2), CAST(N'2024-07-15T22:54:14.4766667' AS DateTime2), 58, 2, 2, 1, 1, CAST(N'2024-07-15T20:54:09.9820000' AS DateTime2), CAST(N'2024-07-22T20:54:09.9820000' AS DateTime2), 7, 0, NULL, NULL, NULL)
GO
INSERT [dbo].[CompanyJobPosts] ([Id], [JobDescription], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [CityId], [AdStartDate], [AdEndDate], [AdDuration], [IsDeleted], [AdName], [Position], [EmailForReceivingApplications]) VALUES (3, N'TEST', CAST(N'2024-07-15T23:02:23.9300000' AS DateTime2), CAST(N'2024-07-15T23:02:23.9300000' AS DateTime2), 58, 1, 1, 3, 1, CAST(N'2024-07-15T21:02:23.8930000' AS DateTime2), CAST(N'2024-07-30T21:02:23.8930000' AS DateTime2), 15, 1, N'Test', N'TEST', NULL)
GO
INSERT [dbo].[CompanyJobPosts] ([Id], [JobDescription], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [CityId], [AdStartDate], [AdEndDate], [AdDuration], [IsDeleted], [AdName], [Position], [EmailForReceivingApplications]) VALUES (4, N'TE', CAST(N'2024-07-15T23:10:58.1900000' AS DateTime2), CAST(N'2024-07-15T23:10:58.1900000' AS DateTime2), 58, 3, 6, 3, 1, CAST(N'2024-07-15T21:10:57.6740000' AS DateTime2), CAST(N'2024-07-22T21:10:57.6740000' AS DateTime2), 7, 1, N'Te', N'Te', N'test@test.com')
GO
SET IDENTITY_INSERT [dbo].[CompanyJobPosts] OFF
GO
SET IDENTITY_INSERT [dbo].[Countries] ON 
GO
INSERT [dbo].[Countries] ([Id], [Name]) VALUES (1, N'Bosnia and Herzegovina')
GO
SET IDENTITY_INSERT [dbo].[Countries] OFF
GO
SET IDENTITY_INSERT [dbo].[JobCategories] ON 
GO
INSERT [dbo].[JobCategories] ([Id], [Name]) VALUES (1, N'IT')
GO
INSERT [dbo].[JobCategories] ([Id], [Name]) VALUES (2, N'Software Development')
GO
INSERT [dbo].[JobCategories] ([Id], [Name]) VALUES (3, N'Data Science')
GO
INSERT [dbo].[JobCategories] ([Id], [Name]) VALUES (4, N'Engineering')
GO
INSERT [dbo].[JobCategories] ([Id], [Name]) VALUES (5, N'Electrical Engineering')
GO
INSERT [dbo].[JobCategories] ([Id], [Name]) VALUES (6, N'Mechanical Engineering')
GO
SET IDENTITY_INSERT [dbo].[JobCategories] OFF
GO
SET IDENTITY_INSERT [dbo].[JobPostStatuses] ON 
GO
INSERT [dbo].[JobPostStatuses] ([Id], [Name]) VALUES (1, N'Active')
GO
INSERT [dbo].[JobPostStatuses] ([Id], [Name]) VALUES (2, N'Closed')
GO
INSERT [dbo].[JobPostStatuses] ([Id], [Name]) VALUES (3, N'Deleted')
GO
SET IDENTITY_INSERT [dbo].[JobPostStatuses] OFF
GO
SET IDENTITY_INSERT [dbo].[JobTypes] ON 
GO
INSERT [dbo].[JobTypes] ([Id], [Name]) VALUES (1, N'Office')
GO
INSERT [dbo].[JobTypes] ([Id], [Name]) VALUES (2, N'Remote')
GO
INSERT [dbo].[JobTypes] ([Id], [Name]) VALUES (3, N'Hybrid')
GO
INSERT [dbo].[JobTypes] ([Id], [Name]) VALUES (4, N'Ostalo')
GO
SET IDENTITY_INSERT [dbo].[JobTypes] OFF
GO
SET IDENTITY_INSERT [dbo].[Photos] ON 
GO
INSERT [dbo].[Photos] ([Id], [PublicId], [Url]) VALUES (1, NULL, N'https://res.cloudinary.com/animated-broccoli/image/upload/v1642909818/ShoppingCart/avuk4toem4by8jno7ekr.jpg')
GO
INSERT [dbo].[Photos] ([Id], [PublicId], [Url]) VALUES (2, NULL, N'https://randomuser.me/api/portraits/men/9.jpg')
GO
INSERT [dbo].[Photos] ([Id], [PublicId], [Url]) VALUES (3, NULL, N'https://randomuser.me/api/portraits/men/96.jpg')
GO
INSERT [dbo].[Photos] ([Id], [PublicId], [Url]) VALUES (4, NULL, N'https://randomuser.me/api/portraits/men/32.jpg')
GO
INSERT [dbo].[Photos] ([Id], [PublicId], [Url]) VALUES (5, NULL, N'https://randomuser.me/api/portraits/men/33.jpg')
GO
INSERT [dbo].[Photos] ([Id], [PublicId], [Url]) VALUES (6, NULL, N'https://randomuser.me/api/portraits/men/99.jpg')
GO
INSERT [dbo].[Photos] ([Id], [PublicId], [Url]) VALUES (7, NULL, N'https://randomuser.me/api/portraits/men/3.jpg')
GO
INSERT [dbo].[Photos] ([Id], [PublicId], [Url]) VALUES (8, NULL, N'https://randomuser.me/api/portraits/men/11.jpg')
GO
INSERT [dbo].[Photos] ([Id], [PublicId], [Url]) VALUES (9, NULL, N'https://randomuser.me/api/portraits/men/35.jpg')
GO
INSERT [dbo].[Photos] ([Id], [PublicId], [Url]) VALUES (10, NULL, N'https://randomuser.me/api/portraits/men/51.jpg')
GO
INSERT [dbo].[Photos] ([Id], [PublicId], [Url]) VALUES (11, NULL, N'https://randomuser.me/api/portraits/men/88.jpg')
GO
INSERT [dbo].[Photos] ([Id], [PublicId], [Url]) VALUES (12, NULL, N'https://randomuser.me/api/portraits/men/17.jpg')
GO
INSERT [dbo].[Photos] ([Id], [PublicId], [Url]) VALUES (13, NULL, N'https://randomuser.me/api/portraits/men/10.jpg')
GO
INSERT [dbo].[Photos] ([Id], [PublicId], [Url]) VALUES (14, NULL, N'https://randomuser.me/api/portraits/men/56.jpg')
GO
INSERT [dbo].[Photos] ([Id], [PublicId], [Url]) VALUES (15, NULL, N'https://randomuser.me/api/portraits/men/65.jpg')
GO
INSERT [dbo].[Photos] ([Id], [PublicId], [Url]) VALUES (16, NULL, N'https://randomuser.me/api/portraits/men/75.jpg')
GO
INSERT [dbo].[Photos] ([Id], [PublicId], [Url]) VALUES (17, NULL, N'https://randomuser.me/api/portraits/men/36.jpg')
GO
INSERT [dbo].[Photos] ([Id], [PublicId], [Url]) VALUES (18, NULL, N'https://randomuser.me/api/portraits/men/40.jpg')
GO
INSERT [dbo].[Photos] ([Id], [PublicId], [Url]) VALUES (19, NULL, N'https://randomuser.me/api/portraits/men/55.jpg')
GO
INSERT [dbo].[Photos] ([Id], [PublicId], [Url]) VALUES (20, NULL, N'https://randomuser.me/api/portraits/men/93.jpg')
GO
INSERT [dbo].[Photos] ([Id], [PublicId], [Url]) VALUES (21, NULL, N'https://randomuser.me/api/portraits/men/7.jpg')
GO
INSERT [dbo].[Photos] ([Id], [PublicId], [Url]) VALUES (22, NULL, N'https://randomuser.me/api/portraits/men/87.jpg')
GO
INSERT [dbo].[Photos] ([Id], [PublicId], [Url]) VALUES (23, NULL, N'https://randomuser.me/api/portraits/men/31.jpg')
GO
INSERT [dbo].[Photos] ([Id], [PublicId], [Url]) VALUES (24, NULL, N'https://randomuser.me/api/portraits/men/16.jpg')
GO
INSERT [dbo].[Photos] ([Id], [PublicId], [Url]) VALUES (25, NULL, N'https://randomuser.me/api/portraits/men/5.jpg')
GO
INSERT [dbo].[Photos] ([Id], [PublicId], [Url]) VALUES (26, NULL, N'https://randomuser.me/api/portraits/men/51.jpg')
GO
INSERT [dbo].[Photos] ([Id], [PublicId], [Url]) VALUES (27, NULL, N'https://randomuser.me/api/portraits/women/93.jpg')
GO
INSERT [dbo].[Photos] ([Id], [PublicId], [Url]) VALUES (28, NULL, N'https://randomuser.me/api/portraits/women/72.jpg')
GO
INSERT [dbo].[Photos] ([Id], [PublicId], [Url]) VALUES (29, NULL, N'https://randomuser.me/api/portraits/women/64.jpg')
GO
INSERT [dbo].[Photos] ([Id], [PublicId], [Url]) VALUES (30, NULL, N'https://randomuser.me/api/portraits/women/12.jpg')
GO
INSERT [dbo].[Photos] ([Id], [PublicId], [Url]) VALUES (31, NULL, N'https://randomuser.me/api/portraits/women/42.jpg')
GO
INSERT [dbo].[Photos] ([Id], [PublicId], [Url]) VALUES (32, NULL, N'https://randomuser.me/api/portraits/women/21.jpg')
GO
INSERT [dbo].[Photos] ([Id], [PublicId], [Url]) VALUES (33, NULL, N'https://randomuser.me/api/portraits/women/9.jpg')
GO
INSERT [dbo].[Photos] ([Id], [PublicId], [Url]) VALUES (34, NULL, N'https://randomuser.me/api/portraits/women/11.jpg')
GO
INSERT [dbo].[Photos] ([Id], [PublicId], [Url]) VALUES (35, NULL, N'https://randomuser.me/api/portraits/women/74.jpg')
GO
INSERT [dbo].[Photos] ([Id], [PublicId], [Url]) VALUES (36, NULL, N'https://randomuser.me/api/portraits/women/17.jpg')
GO
INSERT [dbo].[Photos] ([Id], [PublicId], [Url]) VALUES (37, NULL, N'https://randomuser.me/api/portraits/women/97.jpg')
GO
INSERT [dbo].[Photos] ([Id], [PublicId], [Url]) VALUES (38, NULL, N'https://randomuser.me/api/portraits/women/59.jpg')
GO
INSERT [dbo].[Photos] ([Id], [PublicId], [Url]) VALUES (39, NULL, N'https://randomuser.me/api/portraits/women/62.jpg')
GO
INSERT [dbo].[Photos] ([Id], [PublicId], [Url]) VALUES (40, NULL, N'https://randomuser.me/api/portraits/women/47.jpg')
GO
INSERT [dbo].[Photos] ([Id], [PublicId], [Url]) VALUES (41, NULL, N'https://randomuser.me/api/portraits/women/72.jpg')
GO
INSERT [dbo].[Photos] ([Id], [PublicId], [Url]) VALUES (42, NULL, N'https://randomuser.me/api/portraits/women/50.jpg')
GO
INSERT [dbo].[Photos] ([Id], [PublicId], [Url]) VALUES (43, NULL, N'https://randomuser.me/api/portraits/women/40.jpg')
GO
INSERT [dbo].[Photos] ([Id], [PublicId], [Url]) VALUES (44, NULL, N'https://randomuser.me/api/portraits/women/1.jpg')
GO
INSERT [dbo].[Photos] ([Id], [PublicId], [Url]) VALUES (45, NULL, N'https://randomuser.me/api/portraits/women/13.jpg')
GO
INSERT [dbo].[Photos] ([Id], [PublicId], [Url]) VALUES (46, NULL, N'https://randomuser.me/api/portraits/women/68.jpg')
GO
INSERT [dbo].[Photos] ([Id], [PublicId], [Url]) VALUES (47, NULL, N'https://randomuser.me/api/portraits/women/30.jpg')
GO
INSERT [dbo].[Photos] ([Id], [PublicId], [Url]) VALUES (48, NULL, N'https://randomuser.me/api/portraits/women/71.jpg')
GO
INSERT [dbo].[Photos] ([Id], [PublicId], [Url]) VALUES (49, NULL, N'https://randomuser.me/api/portraits/women/14.jpg')
GO
INSERT [dbo].[Photos] ([Id], [PublicId], [Url]) VALUES (50, NULL, N'https://randomuser.me/api/portraits/women/23.jpg')
GO
INSERT [dbo].[Photos] ([Id], [PublicId], [Url]) VALUES (51, NULL, N'https://randomuser.me/api/portraits/women/20.jpg')
GO
INSERT [dbo].[Photos] ([Id], [PublicId], [Url]) VALUES (52, NULL, NULL)
GO
INSERT [dbo].[Photos] ([Id], [PublicId], [Url]) VALUES (53, NULL, NULL)
GO
INSERT [dbo].[Photos] ([Id], [PublicId], [Url]) VALUES (54, NULL, NULL)
GO
INSERT [dbo].[Photos] ([Id], [PublicId], [Url]) VALUES (55, NULL, NULL)
GO
INSERT [dbo].[Photos] ([Id], [PublicId], [Url]) VALUES (1052, NULL, NULL)
GO
INSERT [dbo].[Photos] ([Id], [PublicId], [Url]) VALUES (1053, NULL, NULL)
GO
INSERT [dbo].[Photos] ([Id], [PublicId], [Url]) VALUES (1054, NULL, NULL)
GO
INSERT [dbo].[Photos] ([Id], [PublicId], [Url]) VALUES (1055, NULL, NULL)
GO
SET IDENTITY_INSERT [dbo].[Photos] OFF
GO
SET IDENTITY_INSERT [dbo].[UserEducations] ON 
GO
INSERT [dbo].[UserEducations] ([Id], [Degree], [University], [InstitutionName], [FieldOfStudy], [EducationStartYear], [EducationEndYear], [UserId]) VALUES (11, N'BA', N'Univ Sarajevo', N'PrMat', N'Mat', 2019, 2024, 55)
GO
INSERT [dbo].[UserEducations] ([Id], [Degree], [University], [InstitutionName], [FieldOfStudy], [EducationStartYear], [EducationEndYear], [UserId]) VALUES (1009, N'BA', N'UNSA', N'PMF', N'MAT', 22, 24, 56)
GO
INSERT [dbo].[UserEducations] ([Id], [Degree], [University], [InstitutionName], [FieldOfStudy], [EducationStartYear], [EducationEndYear], [UserId]) VALUES (1010, N'SRSK', N'PGSA', N'PGSA', N'MAT', 2015, 2025, 56)
GO
INSERT [dbo].[UserEducations] ([Id], [Degree], [University], [InstitutionName], [FieldOfStudy], [EducationStartYear], [EducationEndYear], [UserId]) VALUES (1011, N'nn', N'nn', N'nn', N'nn', 22, 0, 56)
GO
SET IDENTITY_INSERT [dbo].[UserEducations] OFF
GO
SET IDENTITY_INSERT [dbo].[UserJobPosts] ON 
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (1, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 2, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (2, N'Data Scientist', N'Passionate data scientist with expertise in machine learning.', CAST(90000.00 AS Decimal(18, 2)), CAST(N'2022-03-16T09:30:00.0000000' AS DateTime2), CAST(N'2022-03-16T09:30:00.0000000' AS DateTime2), 2, 2, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 2, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (6, N'aa', N'aa', CAST(3.00 AS Decimal(18, 2)), CAST(N'2024-03-20T20:44:58.6066667' AS DateTime2), CAST(N'2024-03-20T20:44:58.6066667' AS DateTime2), 1, 1, 1, 1, N'aa', N'aa', CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), N'aa@aa.com', 1, N'123123', 1, 2, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (7, N'sdad', N'sda', NULL, CAST(N'2024-03-20T21:49:33.1766667' AS DateTime2), CAST(N'2024-03-20T21:49:33.1766667' AS DateTime2), 1, 2, 1, 1, N'ttee', N'tete', CAST(N'2024-03-19T23:00:00.0000000' AS DateTime2), N'ett@tet.com', 0, N'123', 1, 2, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (8, N'sdad', N'rerr', NULL, CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), 1, 1, 1, 1, N'rerer', N'erere', CAST(N'2024-03-19T23:00:00.0000000' AS DateTime2), N'rere@rere.com', 2, N'123', 1, 2, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (10, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 2, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (11, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 2, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (12, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 2, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (13, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 2, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (14, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 2, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (15, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 2, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (16, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 2, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (17, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 2, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (18, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 2, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (19, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 2, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (20, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (21, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (22, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (23, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (24, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (25, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (26, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (27, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (28, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (29, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (30, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (31, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (32, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (33, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (34, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (35, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (36, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (37, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (38, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (39, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (40, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (41, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (42, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (43, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (44, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (45, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (46, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (47, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (48, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (49, N'Data Scientist', N'Passionate data scientist with expertise in machine learning.', CAST(90000.00 AS Decimal(18, 2)), CAST(N'2022-03-16T09:30:00.0000000' AS DateTime2), CAST(N'2022-03-16T09:30:00.0000000' AS DateTime2), 2, 2, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (50, N'aa', N'This is a basic format for a bibliography entry for a book. Depending on the citation style (APA, MLA, Chicago, etc.), the format may vary. Always consult the specific style guide you are using for precise formatting instructions.', CAST(3.00 AS Decimal(18, 2)), CAST(N'2024-03-20T20:44:58.6066667' AS DateTime2), CAST(N'2024-03-20T20:44:58.6066667' AS DateTime2), 1, 1, 1, 1, N'aa', N'aa', CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), N'aa@aa.com', 1, N'123123', 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (51, N'sdad', N'sda', NULL, CAST(N'2024-03-20T21:49:33.1766667' AS DateTime2), CAST(N'2024-03-20T21:49:33.1766667' AS DateTime2), 1, 2, 1, 1, N'ttee', N'tete', CAST(N'2024-03-19T23:00:00.0000000' AS DateTime2), N'ett@tet.com', 0, N'123', 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (52, N'sdad', N'rerr', NULL, CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), 1, 1, 1, 1, N'rerer', N'erere', CAST(N'2024-03-19T23:00:00.0000000' AS DateTime2), N'rere@rere.com', 2, N'123', 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (53, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (54, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (55, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (56, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (57, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (58, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (59, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (60, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (61, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (62, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (63, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (64, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (65, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (66, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (67, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (68, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (69, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (70, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (71, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (72, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (73, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (74, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (75, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (76, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (77, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (78, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (79, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (80, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (81, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (82, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (83, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (84, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (85, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (86, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (87, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (88, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (89, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (90, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (91, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (92, N'Data Scientist', N'Passionate data scientist with expertise in machine learning.', CAST(90000.00 AS Decimal(18, 2)), CAST(N'2022-03-16T09:30:00.0000000' AS DateTime2), CAST(N'2022-03-16T09:30:00.0000000' AS DateTime2), 2, 2, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (93, N'aa', N'aa', CAST(3.00 AS Decimal(18, 2)), CAST(N'2024-03-20T20:44:58.6066667' AS DateTime2), CAST(N'2024-03-20T20:44:58.6066667' AS DateTime2), 1, 1, 1, 1, N'aa', N'aa', CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), N'aa@aa.com', 1, N'123123', 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (94, N'sdad', N'sda', NULL, CAST(N'2024-03-20T21:49:33.1766667' AS DateTime2), CAST(N'2024-03-20T21:49:33.1766667' AS DateTime2), 1, 2, 1, 1, N'ttee', N'tete', CAST(N'2024-03-19T23:00:00.0000000' AS DateTime2), N'ett@tet.com', 0, N'123', 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (95, N'sdad', N'rerr', NULL, CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), 1, 1, 1, 1, N'rerer', N'erere', CAST(N'2024-03-19T23:00:00.0000000' AS DateTime2), N'rere@rere.com', 2, N'123', 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (96, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (97, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (98, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (99, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (100, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (101, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (102, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (103, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (104, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (105, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (106, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (107, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (108, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (109, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (110, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (111, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (112, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (113, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (114, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (115, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (116, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (117, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (118, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (119, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (120, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (121, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (122, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (123, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (124, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (125, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (126, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (127, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (128, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (129, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (130, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (131, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (132, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (133, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (134, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (135, N'Data Scientist', N'Passionate data scientist with expertise in machine learning.', CAST(90000.00 AS Decimal(18, 2)), CAST(N'2022-03-16T09:30:00.0000000' AS DateTime2), CAST(N'2022-03-16T09:30:00.0000000' AS DateTime2), 2, 2, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (136, N'aa', N'aa', CAST(3.00 AS Decimal(18, 2)), CAST(N'2024-03-20T20:44:58.6066667' AS DateTime2), CAST(N'2024-03-20T20:44:58.6066667' AS DateTime2), 1, 1, 1, 1, N'aa', N'aa', CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), N'aa@aa.com', 1, N'123123', 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (137, N'sdad', N'sda', NULL, CAST(N'2024-03-20T21:49:33.1766667' AS DateTime2), CAST(N'2024-03-20T21:49:33.1766667' AS DateTime2), 1, 2, 1, 1, N'ttee', N'tete', CAST(N'2024-03-19T23:00:00.0000000' AS DateTime2), N'ett@tet.com', 0, N'123', 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (138, N'sdad', N'rerr', NULL, CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), 1, 1, 1, 1, N'rerer', N'erere', CAST(N'2024-03-19T23:00:00.0000000' AS DateTime2), N'rere@rere.com', 2, N'123', 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (139, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (140, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (141, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (142, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (143, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (144, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (145, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (146, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (147, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (148, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (149, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (150, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (151, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (152, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (153, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (154, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (155, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (156, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (157, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (158, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (159, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (160, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (161, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (162, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (163, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (164, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (165, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (166, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (167, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (168, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (169, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (170, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (171, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (172, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (173, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (174, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (175, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (176, N'Software Engineer', N'Experienced software engineer with expertise in web development.', CAST(80000.00 AS Decimal(18, 2)), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), CAST(N'2022-03-15T10:00:00.0000000' AS DateTime2), 1, 1, 1, 1, NULL, NULL, CAST(N'2024-03-20T21:06:00.0000000' AS DateTime2), NULL, 0, NULL, 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (177, N'aa', N'aa', NULL, CAST(N'2024-06-30T17:11:38.6966667' AS DateTime2), CAST(N'2024-06-30T17:11:38.6966667' AS DateTime2), 56, 2, 1, 3, N'aa', N'aa', CAST(N'2024-06-01T22:00:00.0000000' AS DateTime2), N'aa@aa.com', 1, N'aa', 2, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 1)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (181, N'Inzinjer', N'Full stack inzinjer', NULL, CAST(N'2024-07-03T11:45:39.2233333' AS DateTime2), CAST(N'2024-07-03T11:45:39.2233333' AS DateTime2), 56, 2, 4, 1, N'emin', N'dukic', CAST(N'1997-07-23T16:00:00.0000000' AS DateTime2), N'test@test.com', 0, N'+38762917495', 1, 1, N'C:\dev\web-shop\web-shop\API\uploads\EminDukic_CV.pdf', 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
INSERT [dbo].[UserJobPosts] ([Id], [Position], [Biography], [Price], [CreatedAt], [UpdatedAt], [SubmittingUserId], [JobTypeId], [JobCategoryId], [JobPostStatusId], [ApplicantFirstName], [ApplicantLastName], [ApplicantDateOfBirth], [ApplicantEmail], [ApplicantGender], [ApplicantPhoneNumber], [CityId], [AdvertisementTypeId], [CvFilePath], [AdDuration], [AdEndDate], [AdStartDate], [IsDeleted]) VALUES (182, N'Inzinjer', N'Full stack inzinjer', NULL, CAST(N'2024-07-03T13:59:43.9866667' AS DateTime2), CAST(N'2024-07-03T13:59:43.9866667' AS DateTime2), 56, 2, 4, 1, N'emin', N'dukic', CAST(N'1997-07-23T16:00:00.0000000' AS DateTime2), N'test@test.com', 0, N'+38762917495', 1, 1, NULL, 7, CAST(N'2024-07-10T14:44:15.9333333' AS DateTime2), CAST(N'2024-07-03T14:43:29.7266667' AS DateTime2), 0)
GO
SET IDENTITY_INSERT [dbo].[UserJobPosts] OFF
GO
ALTER TABLE [dbo].[AspNetUsers] ADD  DEFAULT ((0)) FOR [CityId]
GO
ALTER TABLE [dbo].[AspNetUsers] ADD  DEFAULT (CONVERT([bit],(0))) FOR [IsCompany]
GO
ALTER TABLE [dbo].[CompanyJobPosts] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[CompanyJobPosts] ADD  DEFAULT (getdate()) FOR [UpdatedAt]
GO
ALTER TABLE [dbo].[CompanyJobPosts] ADD  DEFAULT ((1)) FOR [CityId]
GO
ALTER TABLE [dbo].[UserJobPosts] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[UserJobPosts] ADD  DEFAULT (getdate()) FOR [UpdatedAt]
GO
ALTER TABLE [dbo].[UserJobPosts] ADD  DEFAULT ('0001-01-01T00:00:00.0000000') FOR [ApplicantDateOfBirth]
GO
ALTER TABLE [dbo].[UserJobPosts] ADD  DEFAULT ((0)) FOR [ApplicantGender]
GO
ALTER TABLE [dbo].[UserJobPosts] ADD  DEFAULT ((1)) FOR [CityId]
GO
ALTER TABLE [dbo].[UserJobPosts] ADD  DEFAULT ((0)) FOR [AdvertisementTypeId]
GO
ALTER TABLE [dbo].[UserJobPosts] ADD  DEFAULT ((0)) FOR [AdDuration]
GO
ALTER TABLE [dbo].[UserJobPosts] ADD  DEFAULT ('0001-01-01T00:00:00.0000000') FOR [AdEndDate]
GO
ALTER TABLE [dbo].[UserJobPosts] ADD  DEFAULT ('0001-01-01T00:00:00.0000000') FOR [AdStartDate]
GO
ALTER TABLE [dbo].[UserJobPosts] ADD  DEFAULT (CONVERT([bit],(0))) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[ApplicantEducations]  WITH CHECK ADD  CONSTRAINT [FK_ApplicantEducations_UserJobPosts_UserJobPostId] FOREIGN KEY([UserJobPostId])
REFERENCES [dbo].[UserJobPosts] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ApplicantEducations] CHECK CONSTRAINT [FK_ApplicantEducations_UserJobPosts_UserJobPostId]
GO
ALTER TABLE [dbo].[AspNetRoleClaims]  WITH CHECK ADD  CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY([RoleId])
REFERENCES [dbo].[AspNetRoles] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetRoleClaims] CHECK CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId]
GO
ALTER TABLE [dbo].[AspNetUserClaims]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserClaims] CHECK CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[AspNetUserLogins]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserLogins] CHECK CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[AspNetUserRoles]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY([RoleId])
REFERENCES [dbo].[AspNetRoles] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserRoles] CHECK CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId]
GO
ALTER TABLE [dbo].[AspNetUserRoles]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserRoles] CHECK CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[AspNetUsers]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUsers_Cities_CityId] FOREIGN KEY([CityId])
REFERENCES [dbo].[Cities] ([Id])
GO
ALTER TABLE [dbo].[AspNetUsers] CHECK CONSTRAINT [FK_AspNetUsers_Cities_CityId]
GO
ALTER TABLE [dbo].[AspNetUsers]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUsers_Companies_CompanyId] FOREIGN KEY([CompanyId])
REFERENCES [dbo].[Companies] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUsers] CHECK CONSTRAINT [FK_AspNetUsers_Companies_CompanyId]
GO
ALTER TABLE [dbo].[AspNetUsers]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUsers_JobCategories_JobCategoryId] FOREIGN KEY([JobCategoryId])
REFERENCES [dbo].[JobCategories] ([Id])
ON DELETE SET NULL
GO
ALTER TABLE [dbo].[AspNetUsers] CHECK CONSTRAINT [FK_AspNetUsers_JobCategories_JobCategoryId]
GO
ALTER TABLE [dbo].[AspNetUsers]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUsers_JobTypes_JobTypeId] FOREIGN KEY([JobTypeId])
REFERENCES [dbo].[JobTypes] ([Id])
ON DELETE SET NULL
GO
ALTER TABLE [dbo].[AspNetUsers] CHECK CONSTRAINT [FK_AspNetUsers_JobTypes_JobTypeId]
GO
ALTER TABLE [dbo].[AspNetUsers]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUsers_Photos_PhotoId] FOREIGN KEY([PhotoId])
REFERENCES [dbo].[Photos] ([Id])
ON DELETE SET NULL
GO
ALTER TABLE [dbo].[AspNetUsers] CHECK CONSTRAINT [FK_AspNetUsers_Photos_PhotoId]
GO
ALTER TABLE [dbo].[AspNetUserTokens]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserTokens] CHECK CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[Cities]  WITH CHECK ADD  CONSTRAINT [FK_Cities_Countries_CountryId] FOREIGN KEY([CountryId])
REFERENCES [dbo].[Countries] ([Id])
GO
ALTER TABLE [dbo].[Cities] CHECK CONSTRAINT [FK_Cities_Countries_CountryId]
GO
ALTER TABLE [dbo].[Companies]  WITH CHECK ADD  CONSTRAINT [FK_Companies_Cities_CityId] FOREIGN KEY([CityId])
REFERENCES [dbo].[Cities] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Companies] CHECK CONSTRAINT [FK_Companies_Cities_CityId]
GO
ALTER TABLE [dbo].[CompanyJobPosts]  WITH CHECK ADD  CONSTRAINT [FK_CompanyJobPosts_AspNetUsers_SubmittingUserId] FOREIGN KEY([SubmittingUserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
GO
ALTER TABLE [dbo].[CompanyJobPosts] CHECK CONSTRAINT [FK_CompanyJobPosts_AspNetUsers_SubmittingUserId]
GO
ALTER TABLE [dbo].[CompanyJobPosts]  WITH CHECK ADD  CONSTRAINT [FK_CompanyJobPosts_Cities_CityId] FOREIGN KEY([CityId])
REFERENCES [dbo].[Cities] ([Id])
GO
ALTER TABLE [dbo].[CompanyJobPosts] CHECK CONSTRAINT [FK_CompanyJobPosts_Cities_CityId]
GO
ALTER TABLE [dbo].[CompanyJobPosts]  WITH CHECK ADD  CONSTRAINT [FK_CompanyJobPosts_JobCategories_JobCategoryId] FOREIGN KEY([JobCategoryId])
REFERENCES [dbo].[JobCategories] ([Id])
GO
ALTER TABLE [dbo].[CompanyJobPosts] CHECK CONSTRAINT [FK_CompanyJobPosts_JobCategories_JobCategoryId]
GO
ALTER TABLE [dbo].[CompanyJobPosts]  WITH CHECK ADD  CONSTRAINT [FK_CompanyJobPosts_JobPostStatuses_JobPostStatusId] FOREIGN KEY([JobPostStatusId])
REFERENCES [dbo].[JobPostStatuses] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[CompanyJobPosts] CHECK CONSTRAINT [FK_CompanyJobPosts_JobPostStatuses_JobPostStatusId]
GO
ALTER TABLE [dbo].[CompanyJobPosts]  WITH CHECK ADD  CONSTRAINT [FK_CompanyJobPosts_JobTypes_JobTypeId] FOREIGN KEY([JobTypeId])
REFERENCES [dbo].[JobTypes] ([Id])
GO
ALTER TABLE [dbo].[CompanyJobPosts] CHECK CONSTRAINT [FK_CompanyJobPosts_JobTypes_JobTypeId]
GO
ALTER TABLE [dbo].[UserEducations]  WITH CHECK ADD  CONSTRAINT [FK_UserEducations_AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[UserEducations] CHECK CONSTRAINT [FK_UserEducations_AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[UserJobPosts]  WITH CHECK ADD  CONSTRAINT [FK_UserJobPosts_AdvertisementTypes_AdvertisementTypeId] FOREIGN KEY([AdvertisementTypeId])
REFERENCES [dbo].[AdvertisementTypes] ([Id])
GO
ALTER TABLE [dbo].[UserJobPosts] CHECK CONSTRAINT [FK_UserJobPosts_AdvertisementTypes_AdvertisementTypeId]
GO
ALTER TABLE [dbo].[UserJobPosts]  WITH CHECK ADD  CONSTRAINT [FK_UserJobPosts_AspNetUsers_SubmittingUserId] FOREIGN KEY([SubmittingUserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
GO
ALTER TABLE [dbo].[UserJobPosts] CHECK CONSTRAINT [FK_UserJobPosts_AspNetUsers_SubmittingUserId]
GO
ALTER TABLE [dbo].[UserJobPosts]  WITH CHECK ADD  CONSTRAINT [FK_UserJobPosts_Cities_CityId] FOREIGN KEY([CityId])
REFERENCES [dbo].[Cities] ([Id])
GO
ALTER TABLE [dbo].[UserJobPosts] CHECK CONSTRAINT [FK_UserJobPosts_Cities_CityId]
GO
ALTER TABLE [dbo].[UserJobPosts]  WITH CHECK ADD  CONSTRAINT [FK_UserJobPosts_JobCategories_JobCategoryId] FOREIGN KEY([JobCategoryId])
REFERENCES [dbo].[JobCategories] ([Id])
GO
ALTER TABLE [dbo].[UserJobPosts] CHECK CONSTRAINT [FK_UserJobPosts_JobCategories_JobCategoryId]
GO
ALTER TABLE [dbo].[UserJobPosts]  WITH CHECK ADD  CONSTRAINT [FK_UserJobPosts_JobPostStatuses_JobPostStatusId] FOREIGN KEY([JobPostStatusId])
REFERENCES [dbo].[JobPostStatuses] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[UserJobPosts] CHECK CONSTRAINT [FK_UserJobPosts_JobPostStatuses_JobPostStatusId]
GO
ALTER TABLE [dbo].[UserJobPosts]  WITH CHECK ADD  CONSTRAINT [FK_UserJobPosts_JobTypes_JobTypeId] FOREIGN KEY([JobTypeId])
REFERENCES [dbo].[JobTypes] ([Id])
GO
ALTER TABLE [dbo].[UserJobPosts] CHECK CONSTRAINT [FK_UserJobPosts_JobTypes_JobTypeId]
GO

USE [DMU]
GO
/****** Object:  Table [dbo].[ColumnType]    Script Date: 03/02/2011 16:58:01 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ColumnType](
	[Id] [int] NOT NULL,
	[TypeName] [nchar](10) NULL,
 CONSTRAINT [PK_ColumnType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AnnotationType]    Script Date: 03/02/2011 16:58:01 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[AnnotationType](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](50) NULL,
 CONSTRAINT [PK_AnnotationType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[DatasetGroup]    Script Date: 03/02/2011 16:58:01 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DatasetGroup](
	[Id] [int] NOT NULL,
	[Title] [nvarchar](50) NULL,
	[Description] [nvarchar](255) NULL,
 CONSTRAINT [PK_ElementGroups] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DatasetProgram]    Script Date: 03/02/2011 16:58:01 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[DatasetProgram](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Description] [varchar](255) NULL,
 CONSTRAINT [PK_DatasetProgram] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[SchemaInfo]    Script Date: 03/02/2011 16:58:01 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[SchemaInfo](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ParentId] [int] NULL,
	[Name] [varchar](50) NULL,
	[ValidBegin] [datetime] NULL,
	[ValidEnd] [datetime] NULL,
 CONSTRAINT [PK_Schema] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[SchemaColumn]    Script Date: 03/02/2011 16:58:01 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[SchemaColumn](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SchemaId] [int] NOT NULL,
	[Name] [varchar](50) NULL,
	[OrigName] [varchar](50) NULL,
	[ColOrder] [nchar](10) NULL,
	[Type] [int] NULL,
	[GroupId] [smallint] NULL,
	[SpatialGeoType] [smallint] NULL,
	[TemporalType] [smallint] NULL,
	[TemporalEndingType] [smallint] NULL,
	[DateFormat] [varchar](50) NULL,
	[ValidBegin] [datetime] NULL,
	[ValidEnd] [datetime] NULL,
 CONSTRAINT [PK_SchemaColumn] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Dataset]    Script Date: 03/02/2011 16:58:01 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Dataset](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[GroupId] [int] NULL,
	[ProgramId] [int] NULL,
	[Title] [nvarchar](1024) NULL,
	[Description] [nvarchar](max) NULL,
	[SampleDescription] [nvarchar](1024) NULL,
	[TimeRangeBegin] [datetime] NULL,
	[TimeRangeEnd] [datetime] NULL,
	[CreatedTime] [datetime] NULL,
 CONSTRAINT [PK_Dataset] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TableInfo]    Script Date: 03/02/2011 16:58:01 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[TableInfo](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ParentId] [int] NULL,
	[SchemaId] [int] NOT NULL,
	[DatasetId] [int] NOT NULL,
	[TableDescription] [varchar](255) NULL,
	[Created] [datetime] NOT NULL,
	[ValidBegin] [datetime2](7) NOT NULL,
	[ValidEnd] [datetime2](7) NULL,
	[Storage] [xml] NULL,
 CONSTRAINT [PK_TableName] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
SET ARITHABORT ON
SET CONCAT_NULL_YIELDS_NULL ON
SET QUOTED_IDENTIFIER ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
SET NUMERIC_ROUNDABORT OFF
CREATE PRIMARY XML INDEX [XML_IX_TableInfo] ON [dbo].[TableInfo] 
(
	[Storage]
)WITH (PAD_INDEX  = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
GO
/****** Object:  Table [dbo].[DatasetParent]    Script Date: 03/02/2011 16:58:01 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DatasetParent](
	[Id] [int] NOT NULL,
	[Parent] [int] NOT NULL,
 CONSTRAINT [PK_DatasetParent] PRIMARY KEY CLUSTERED 
(
	[Id] ASC,
	[Parent] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TemporalPoint]    Script Date: 03/02/2011 16:58:01 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TemporalPoint](
	[TableId] [int] NOT NULL,
	[RowNr] [int] NOT NULL,
	[TemporalGroup] [tinyint] NOT NULL,
	[TimePoint] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_TemporalPoint] PRIMARY KEY CLUSTERED 
(
	[TableId] ASC,
	[RowNr] ASC,
	[TemporalGroup] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_TemporalPoint] ON [dbo].[TemporalPoint] 
(
	[TimePoint] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TemporalLength]    Script Date: 03/02/2011 16:58:01 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TemporalLength](
	[TableId] [int] NOT NULL,
	[RowNr] [int] NOT NULL,
	[TemporalGroup] [tinyint] NOT NULL,
	[TimeLength] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_TemporalLength] PRIMARY KEY CLUSTERED 
(
	[TableId] ASC,
	[RowNr] ASC,
	[TemporalGroup] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TemporalInterval]    Script Date: 03/02/2011 16:58:01 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TemporalInterval](
	[TableId] [int] NOT NULL,
	[RowNr] [int] NOT NULL,
	[TemporalGroup] [tinyint] NOT NULL,
	[TimeBegin] [datetime2](7) NOT NULL,
	[TimeEnd] [datetime2](7) NULL,
 CONSTRAINT [PK_TemporalInterval] PRIMARY KEY CLUSTERED 
(
	[TableId] ASC,
	[RowNr] ASC,
	[TemporalGroup] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_TemporalInterval] ON [dbo].[TemporalInterval] 
(
	[TimeBegin] ASC,
	[TimeEnd] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_TemporalInterval_1] ON [dbo].[TemporalInterval] 
(
	[TimeEnd] ASC,
	[TimeBegin] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TableInfoParent]    Script Date: 03/02/2011 16:58:01 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TableInfoParent](
	[Id] [int] NOT NULL,
	[Parent] [int] NOT NULL,
 CONSTRAINT [PK_TableInfoParent] PRIMARY KEY CLUSTERED 
(
	[Id] ASC,
	[Parent] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SpatialInfo]    Script Date: 03/02/2011 16:58:01 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SpatialInfo](
	[TableId] [int] NOT NULL,
	[RowNr] [int] NOT NULL,
	[SpatialGroup] [tinyint] NOT NULL,
	[Location] [geography] NOT NULL,
 CONSTRAINT [PK_SpatialInfo] PRIMARY KEY CLUSTERED 
(
	[TableId] ASC,
	[RowNr] ASC,
	[SpatialGroup] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE SPATIAL INDEX [SPATIAL_SpatialInfo] ON [dbo].[SpatialInfo] 
(
	[Location]
)USING  GEOGRAPHY_GRID 
WITH (
GRIDS =(LEVEL_1 = MEDIUM,LEVEL_2 = MEDIUM,LEVEL_3 = MEDIUM,LEVEL_4 = MEDIUM), 
CELLS_PER_OBJECT = 16, PAD_INDEX  = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Annotation]    Script Date: 03/02/2011 16:58:01 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Annotation](
	[TableId] [int] NOT NULL,
	[ColumnId] [int] NOT NULL,
	[RowId] [int] NOT NULL,
	[Annotation] [varchar](255) NOT NULL,
	[Type] [int] NOT NULL,
	[Date] [datetime] NOT NULL,
 CONSTRAINT [PK_Annotation] PRIMARY KEY CLUSTERED 
(
	[TableId] ASC,
	[ColumnId] ASC,
	[RowId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE NONCLUSTERED INDEX [IX_Annotation] ON [dbo].[Annotation] 
(
	[TableId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Default [DF_TableInfo_Created]    Script Date: 03/02/2011 16:58:01 ******/
ALTER TABLE [dbo].[TableInfo] ADD  CONSTRAINT [DF_TableInfo_Created]  DEFAULT (getdate()) FOR [Created]
GO
/****** Object:  ForeignKey [FK_Annotation_AnnotationType]    Script Date: 03/02/2011 16:58:01 ******/
ALTER TABLE [dbo].[Annotation]  WITH CHECK ADD  CONSTRAINT [FK_Annotation_AnnotationType] FOREIGN KEY([Type])
REFERENCES [dbo].[AnnotationType] ([Id])
GO
ALTER TABLE [dbo].[Annotation] CHECK CONSTRAINT [FK_Annotation_AnnotationType]
GO
/****** Object:  ForeignKey [FK_Annotation_Table]    Script Date: 03/02/2011 16:58:01 ******/
ALTER TABLE [dbo].[Annotation]  WITH CHECK ADD  CONSTRAINT [FK_Annotation_Table] FOREIGN KEY([TableId])
REFERENCES [dbo].[TableInfo] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Annotation] CHECK CONSTRAINT [FK_Annotation_Table]
GO
/****** Object:  ForeignKey [FK_Dataset_DatasetGroup]    Script Date: 03/02/2011 16:58:01 ******/
ALTER TABLE [dbo].[Dataset]  WITH CHECK ADD  CONSTRAINT [FK_Dataset_DatasetGroup] FOREIGN KEY([GroupId])
REFERENCES [dbo].[DatasetGroup] ([Id])
GO
ALTER TABLE [dbo].[Dataset] CHECK CONSTRAINT [FK_Dataset_DatasetGroup]
GO
/****** Object:  ForeignKey [FK_Dataset_DatasetProgram]    Script Date: 03/02/2011 16:58:01 ******/
ALTER TABLE [dbo].[Dataset]  WITH CHECK ADD  CONSTRAINT [FK_Dataset_DatasetProgram] FOREIGN KEY([ProgramId])
REFERENCES [dbo].[DatasetProgram] ([Id])
GO
ALTER TABLE [dbo].[Dataset] CHECK CONSTRAINT [FK_Dataset_DatasetProgram]
GO
/****** Object:  ForeignKey [FK_DatasetParent_Dataset]    Script Date: 03/02/2011 16:58:01 ******/
ALTER TABLE [dbo].[DatasetParent]  WITH CHECK ADD  CONSTRAINT [FK_DatasetParent_Dataset] FOREIGN KEY([Id])
REFERENCES [dbo].[Dataset] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[DatasetParent] CHECK CONSTRAINT [FK_DatasetParent_Dataset]
GO
/****** Object:  ForeignKey [FK_DatasetParent_Dataset1]    Script Date: 03/02/2011 16:58:01 ******/
ALTER TABLE [dbo].[DatasetParent]  WITH CHECK ADD  CONSTRAINT [FK_DatasetParent_Dataset1] FOREIGN KEY([Parent])
REFERENCES [dbo].[Dataset] ([Id])
GO
ALTER TABLE [dbo].[DatasetParent] CHECK CONSTRAINT [FK_DatasetParent_Dataset1]
GO
/****** Object:  ForeignKey [FK_SchemaColumn_ColumnType]    Script Date: 03/02/2011 16:58:01 ******/
ALTER TABLE [dbo].[SchemaColumn]  WITH CHECK ADD  CONSTRAINT [FK_SchemaColumn_ColumnType] FOREIGN KEY([Type])
REFERENCES [dbo].[ColumnType] ([Id])
GO
ALTER TABLE [dbo].[SchemaColumn] CHECK CONSTRAINT [FK_SchemaColumn_ColumnType]
GO
/****** Object:  ForeignKey [FK_SchemaColumn_SchemaColumn]    Script Date: 03/02/2011 16:58:01 ******/
ALTER TABLE [dbo].[SchemaColumn]  WITH CHECK ADD  CONSTRAINT [FK_SchemaColumn_SchemaColumn] FOREIGN KEY([SchemaId])
REFERENCES [dbo].[SchemaInfo] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[SchemaColumn] CHECK CONSTRAINT [FK_SchemaColumn_SchemaColumn]
GO
/****** Object:  ForeignKey [FK_SpatialInfo_TableInfo]    Script Date: 03/02/2011 16:58:01 ******/
ALTER TABLE [dbo].[SpatialInfo]  WITH CHECK ADD  CONSTRAINT [FK_SpatialInfo_TableInfo] FOREIGN KEY([TableId])
REFERENCES [dbo].[TableInfo] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[SpatialInfo] CHECK CONSTRAINT [FK_SpatialInfo_TableInfo]
GO
/****** Object:  ForeignKey [FK_TableInfo_Dataset]    Script Date: 03/02/2011 16:58:01 ******/
ALTER TABLE [dbo].[TableInfo]  WITH CHECK ADD  CONSTRAINT [FK_TableInfo_Dataset] FOREIGN KEY([DatasetId])
REFERENCES [dbo].[Dataset] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[TableInfo] CHECK CONSTRAINT [FK_TableInfo_Dataset]
GO
/****** Object:  ForeignKey [FK_TableInfo_SchemaInfo]    Script Date: 03/02/2011 16:58:01 ******/
ALTER TABLE [dbo].[TableInfo]  WITH CHECK ADD  CONSTRAINT [FK_TableInfo_SchemaInfo] FOREIGN KEY([SchemaId])
REFERENCES [dbo].[SchemaInfo] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[TableInfo] CHECK CONSTRAINT [FK_TableInfo_SchemaInfo]
GO
/****** Object:  ForeignKey [FK_TableInfoParent_TableInfo]    Script Date: 03/02/2011 16:58:01 ******/
ALTER TABLE [dbo].[TableInfoParent]  WITH CHECK ADD  CONSTRAINT [FK_TableInfoParent_TableInfo] FOREIGN KEY([Id])
REFERENCES [dbo].[TableInfo] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[TableInfoParent] CHECK CONSTRAINT [FK_TableInfoParent_TableInfo]
GO
/****** Object:  ForeignKey [FK_TableInfoParent_TableInfo1]    Script Date: 03/02/2011 16:58:01 ******/
ALTER TABLE [dbo].[TableInfoParent]  WITH CHECK ADD  CONSTRAINT [FK_TableInfoParent_TableInfo1] FOREIGN KEY([Parent])
REFERENCES [dbo].[TableInfo] ([Id])
GO
ALTER TABLE [dbo].[TableInfoParent] CHECK CONSTRAINT [FK_TableInfoParent_TableInfo1]
GO
/****** Object:  ForeignKey [FK_TemporalInterval_TableInfo]    Script Date: 03/02/2011 16:58:01 ******/
ALTER TABLE [dbo].[TemporalInterval]  WITH CHECK ADD  CONSTRAINT [FK_TemporalInterval_TableInfo] FOREIGN KEY([TableId])
REFERENCES [dbo].[TableInfo] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[TemporalInterval] CHECK CONSTRAINT [FK_TemporalInterval_TableInfo]
GO
/****** Object:  ForeignKey [FK_TemporalLength_TableInfo]    Script Date: 03/02/2011 16:58:01 ******/
ALTER TABLE [dbo].[TemporalLength]  WITH CHECK ADD  CONSTRAINT [FK_TemporalLength_TableInfo] FOREIGN KEY([TableId])
REFERENCES [dbo].[TableInfo] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[TemporalLength] CHECK CONSTRAINT [FK_TemporalLength_TableInfo]
GO
/****** Object:  ForeignKey [FK_TemporalPoint_TableInfo]    Script Date: 03/02/2011 16:58:01 ******/
ALTER TABLE [dbo].[TemporalPoint]  WITH CHECK ADD  CONSTRAINT [FK_TemporalPoint_TableInfo] FOREIGN KEY([TableId])
REFERENCES [dbo].[TableInfo] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[TemporalPoint] CHECK CONSTRAINT [FK_TemporalPoint_TableInfo]
GO

INSERT INTO DatasetProgram (Description) VALUES ( 'BioBasis' )
INSERT INTO DatasetProgram (Description) VALUES ( 'GeoBasis' )
INSERT INTO DatasetProgram (Description) VALUES ( 'ClimateBasis' )
INSERT INTO DatasetProgram (Description) VALUES ( 'MarineBasis' )

INSERT INTO Dataset (
	Title,
	Description,
	TimeRangeBegin,
	TimeRangeEnd,
	CreatedTime
) VALUES ( 
	/* Title - nvarchar(1024) */ N'DMU',
	/* Description - nvarchar(max) */ N'Root for DataSets',
	/* TimeRangeBegin - datetime */ '2011-3-2 17:2:39.852',
	/* TimeRangeEnd - datetime */ '2011-3-2 17:2:39.852',
	/* CreatedTime - datetime */ '2011-3-2 17:2:39.852' ) 
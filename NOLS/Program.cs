// See https://aka.ms/new-console-template for more information
using LinqToDB;
using LinqToDB.Data;
using LinqToDB.DataProvider;
using LinqToDB.DataProvider.DB2iSeries;
using System.Data;


Console.WriteLine("Hello, World!");

//var test = DB2iSeriesTools.GetDataProvider(DB2iSeriesProviderName.DB2_ODBC_71_GAS);

//var connection = (IDbConnection)test.CreateConnection("Driver=iSeries Access ODBC Driver;System=CZLSP001;Uid=CLIPSVRPRD;Pwd=Auticko1;DBQ=LPCZPFIL,LPCZPOBJ;NAM=0;UNICODESQL=1;MAXDECSCALE=63;MAXDECPREC=63;GRAPHIC=1;MAPDECIMALFLOATDESCRIBE=3;MAXFIELDLEN=2097152;ALLOWUNSCHAR=1;COMPRESSION=1;SORTTYPE=2;LANGUAGEID=CSY");

//connection.Open();

//connection.Close();

var cnn = new DataConnection((IDataProvider)DB2iSeriesTools.GetDataProvider(DB2iSeriesProviderName.DB2_ODBC_71_GAS), "Driver=iSeries Access ODBC Driver;System=CZLSP001;Uid=CLIPSVRPRD;Pwd=Auticko1;DBQ=LPCZPFIL,LPCZPOBJ;NAM=0;UNICODESQL=1;MAXDECSCALE=63;MAXDECPREC=63;GRAPHIC=1;MAPDECIMALFLOATDESCRIBE=3;MAXFIELDLEN=2097152;ALLOWUNSCHAR=1;COMPRESSION=1;SORTTYPE=2;LANGUAGEID=CSY");



var result = cnn.Query<int>("Select count (*) from LPCZPFIL.PF30");

Console.WriteLine(result.First());
Imports System.Web
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.Data

' To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line.
<System.Web.Script.Services.ScriptService()> _
<WebService(Namespace:="http://tempuri.org/")> _
<WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Public Class wsdashboard
    Inherits System.Web.Services.WebService
    <WebMethod()> _
    Public Function hoy() As List(Of [Datos])

        Dim result As List(Of [Datos]) = New List(Of Datos)()
        Dim StrEncabezado As String = "SELECT COUNT(*) AS total from ENC_FACTURA  WHERE CONVERT(date, Fecha) = CONVERT(date, GETDATE())"
        Dim TablaEncabezado As DataTable = manipular.Login(StrEncabezado)

        For i = 0 To TablaEncabezado.Rows.Count - 1
            For ii = 0 To 1
                Dim Elemento As New Datos
                Elemento.total = TablaEncabezado.Rows(i).Item("total")
                result.Add(Elemento)

                ii = ii + 1
            Next
        Next

        Return result
    End Function

    <WebMethod()> _
    Public Function EstaSemana() As List(Of [Datos])

        Dim result As List(Of [Datos]) = New List(Of Datos)()
        Dim StrEncabezado As String = "SELECT COUNT(*) AS total from ENC_FACTURA  WHERE Fecha >=  DATEADD(day,-7, GETDATE()) "
        Dim TablaEncabezado As DataTable = manipular.Login(StrEncabezado)

        For i = 0 To TablaEncabezado.Rows.Count - 1
            For ii = 0 To 1
                Dim Elemento As New Datos
                Elemento.total = TablaEncabezado.Rows(i).Item("total")
                result.Add(Elemento)

                ii = ii + 1
            Next
        Next

        Return result
    End Function

    <WebMethod()> _
    Public Function EsteMes() As List(Of [Datos])

        Dim result As List(Of [Datos]) = New List(Of Datos)()
        Dim StrEncabezado As String = "SELECT COUNT(*) AS total from ENC_FACTURA   WHERE Fecha BETWEEN DATEADD(mm,DATEDIFF(mm,0,GETDATE()),0) AND DATEADD(ms,-3,DATEADD(mm,0,DATEADD(mm,DATEDIFF(mm,0,GETDATE())+1,0)))"
        Dim TablaEncabezado As DataTable = manipular.Login(StrEncabezado)

        For i = 0 To TablaEncabezado.Rows.Count - 1
            For ii = 0 To 1
                Dim Elemento As New Datos
                Elemento.total = TablaEncabezado.Rows(i).Item("total")
                result.Add(Elemento)

                ii = ii + 1
            Next
        Next

        Return result
    End Function

    'metodo utilizado para obtener las ultimas 5 ventas 
    <WebMethod()> _
    Public Function top5ventas() As List(Of [Datos])

        Dim result As List(Of [Datos]) = New List(Of Datos)()
        Dim StrEncabezado As String = "select top 5 e.id_enc, c.Nom_clt, e.Total_Factura, CONVERT(DATE,e.Fecha) AS Fecha, RIGHT(CONVERT(DATETIME, e.Fecha, 108),8) AS hora    from ENC_FACTURA e " &
                                       "join CLiente  c " &
                                       " on c.Id_Clt  = e.Id_Clt " &
                                       " order by e.id_enc desc"
        Dim TablaEncabezado As DataTable = manipular.Login(StrEncabezado)

        For i = 0 To TablaEncabezado.Rows.Count - 1
            For ii = 0 To 1
                Dim Elemento As New Datos
                Elemento.cliente = TablaEncabezado.Rows(i).Item("Nom_clt")
                Elemento.total = TablaEncabezado.Rows(i).Item("Total_Factura")
                Elemento.fecha = TablaEncabezado.Rows(i).Item("Fecha")
                Elemento.hora = TablaEncabezado.Rows(i).Item("hora")
                result.Add(Elemento)

                ii = ii + 1
            Next
        Next

        Return result
    End Function

    Public Class Datos
        Public total As String
        Public cliente As String
        Public fecha As String
        Public hora As String
    End Class
End Class
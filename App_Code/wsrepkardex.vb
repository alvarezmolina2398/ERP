Imports System.Web
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.Data

' To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line.
<System.Web.Script.Services.ScriptService()> _
<WebService(Namespace:="http://tempuri.org/")> _
<WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Public Class wsrepkardex
    Inherits System.Web.Services.WebService
    'consultar detalle 
    <WebMethod()> _
    Public Function consultar(ByVal fechaIni As String, ByVal fechaFin As String, ByVal prod As Integer, ByVal bod As Integer) As List(Of [Datos])
        Dim result As List(Of [Datos]) = New List(Of Datos)()


        Dim StrEncabezado As String = "EXECUTE dbo.VeSaldosAticuloKardex @id_art = " & prod & ", @idBod = " & bod & ", @fecha1 = '" & fechaIni & "', @fecha2 = '" & fechaFin & "'"
        Dim TablaEncabezado As DataTable = manipular.Login(StrEncabezado)


        For Each row As DataRow In TablaEncabezado.Rows
            Dim Elemento As New Datos
            Elemento.articulo = row(0).ToString
            Elemento.documento = row(4).ToString & "-" & row(1).ToString
            Elemento.fecha = row(7).ToString
            Elemento.bodega = row(2).ToString
            Elemento.cini = row(5).ToString
            Dim Signo As String
            If row(4).ToString = "F" Then
                Signo = "-"
            Else
                Signo = ""
            End If
            Elemento.cmov = Signo & row(3).ToString
            Elemento.cfin = row(6).ToString
            Elemento.usuario = row(8).ToString
            result.Add(Elemento)
        Next
        Return result
    End Function

    Public Class Datos
        Public articulo As String
        Public documento As String
        Public fecha As String
        Public bodega As String
        Public cini As String
        Public cmov As String
        Public cfin As String
        Public usuario As String
    End Class
End Class
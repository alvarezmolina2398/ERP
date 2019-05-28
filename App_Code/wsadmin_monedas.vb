Imports System.Data
Imports System.Web
Imports System.Web.Services
Imports System.Web.Services.Protocols

' Para permitir que se llame a este servicio web desde un script, usando ASP.NET AJAX, quite la marca de comentario de la línea siguiente.
<System.Web.Script.Services.ScriptService()>
<WebService(Namespace:="http://tempuri.org/")> _
<WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Public Class wsadmin_monedas
    Inherits System.Web.Services.WebService

    'Metodo para cargar  los datos
    <WebMethod()>
    Public Function ObtenerDatos() As List(Of datos)
        Dim SQL As String = "SELECT * FROM  [MONEDAS] WHERE estado = 1 "

        Dim result As List(Of [datos]) = New List(Of datos)()
        Dim TablaEncabezado As DataTable = manipular.ObtenerDatos(SQL)

        For i = 0 To TablaEncabezado.Rows.Count - 1
            For ii = 0 To 1
                Dim Elemento As New datos
                Elemento.id = TablaEncabezado.Rows(i).Item("ID_MONEDA")
                Elemento.descripcion = TablaEncabezado.Rows(i).Item("DESCRIPCION").ToString

                result.Add(Elemento)
                ii = ii + 1
            Next
        Next

        Return result

    End Function



    Public Class datos
        Public id As Integer
        Public descripcion As String
    End Class

End Class
Imports System.Web
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.Data

' To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line.
<System.Web.Script.Services.ScriptService()> _
<WebService(Namespace:="http://tempuri.org/")> _
<WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Public Class wsclasificacionArt
    Inherits System.Web.Services.WebService
    'Declaracion de variables a utilizar
    Public Class muestra_estado
        Public id_clasif As String
        Public id_nombre As String
        Public id_descripcion As String
        Public id_observacion As String
    End Class

    'metodo utilizado para mostrar la clasificacion de articulos
    <WebMethod()> _
    Public Function ObtenerClasificacion() As List(Of muestra_estado)
        Dim result As List(Of muestra_estado) = New List(Of muestra_estado)()
        Dim StrEncabezado As String = "select * from clasificacionarticulo where estado = 1"
        Dim TablaEncabezado As DataTable = manipular.Login(StrEncabezado)

        If TablaEncabezado.Rows.Count > 0 Then
            For i = 0 To TablaEncabezado.Rows.Count - 1
                Dim Elemento As New muestra_estado
                Elemento.id_clasif = TablaEncabezado.Rows(i).Item("id_tipo_art").ToString()
                Elemento.id_nombre = TablaEncabezado.Rows(i).Item("nom_tipo_art").ToString()
                Elemento.id_descripcion = TablaEncabezado.Rows(i).Item("des_tipo_art").ToString()
                Elemento.id_observacion = TablaEncabezado.Rows(i).Item("observ_tipo_art").ToString()
                result.Add(Elemento)
            Next
        End If

        Return result

    End Function

    'metodo utilizado para insertar una clasificacion de articulos
    <WebMethod()> _
    Public Function insertar(ByVal nombre As String, ByVal descripcion As String, ByVal observacion As String) As Boolean
        Dim result As Boolean = False
        Dim strinsert As String = "insert into dbo.clasificacionarticulo " &
                                    "(nom_tipo_art,des_tipo_art,observ_tipo_art,estado) " &
                                    " values " &
                                    " ('" & nombre & "', '" & descripcion & "', '" & observacion & "',1)"
        result = manipular.EjecutaTransaccion1(strinsert)
        Return result
    End Function

    'metodo utilizado para actualizar la clasificacion de articulo
    <WebMethod()> _
    Public Function actualizar(ByVal nombre As String, ByVal descripcion As String, ByVal observacion As String, ByVal id As Integer) As Boolean
        Dim result As Boolean = False
        Dim actualizarClasificacion = "UPDATE clasificacionarticulo SET Nom_Tipo_Art='" & nombre & "', Des_Tipo_Art = '" & descripcion & "', Observ_Tipo_Art= '" & observacion & "', Estado= 1 WHERE Id_Tipo_Art = " & id
        result = manipular.EjecutaTransaccion1(actualizarClasificacion)
        Return result
    End Function

    'metodo utilizado para actualizar la clasificacion de articulo
    <WebMethod()> _
    Public Function cambiarEstado(ByVal id As Integer) As Boolean
        Dim result As Boolean = False
        Dim actualizarClasificacion = "UPDATE clasificacionarticulo SET estado = 0 WHERE Id_Tipo_Art = " & id
        result = manipular.EjecutaTransaccion1(actualizarClasificacion)
        Return result
    End Function

End Class
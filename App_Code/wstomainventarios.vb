Imports System.Data
Imports System.Web
Imports System.Web.Services
Imports System.Web.Services.Protocols

' Para permitir que se llame a este servicio web desde un script, usando ASP.NET AJAX, quite la marca de comentario de la línea siguiente.
<System.Web.Script.Services.ScriptService()>
<WebService(Namespace:="http://tempuri.org/")> _
<WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Public Class wstomainventarios
    Inherits System.Web.Services.WebService


    <WebMethod()>
    Public Function ObtenerDatos() As List(Of datos)
        Dim SQL As String = "SELECT  t.id_enc,t.observaciones,convert(varchar,t.fecha,101) as fecha,convert(varchar,t.fecha,24) as hora,s.id_suc,t.estado,t.usuario,s.descripcion as sucursal FROM  [ENC_TOMA_INVENTARIO]  t  INNER JOIN   [SUCURSALES] s on s.id_suc = t.id_suc  where t.estado = 1"

        Dim result As List(Of [datos]) = New List(Of datos)()
        Dim TablaEncabezado As DataTable = manipular.ObtenerDatos(SQL)

        For i = 0 To TablaEncabezado.Rows.Count - 1
            For ii = 0 To 1
                Dim Elemento As New datos
                Elemento.id = TablaEncabezado.Rows(i).Item("id_enc")
                Elemento.observaciones = TablaEncabezado.Rows(i).Item("observaciones").ToString
                Elemento.fecha = TablaEncabezado.Rows(i).Item("fecha").ToString
                Elemento.hora = TablaEncabezado.Rows(i).Item("hora")
                Elemento.Sucursal = TablaEncabezado.Rows(i).Item("sucursal")
                Elemento.idsucursal = TablaEncabezado.Rows(i).Item("id_suc")
                Elemento.usuario = TablaEncabezado.Rows(i).Item("usuario")
                result.Add(Elemento)
                ii = ii + 1
            Next
        Next

        Return result

    End Function



    <WebMethod()>
    Public Function ObtenerListadoProductos(ByVal bodega As Integer, ByVal sucursal As Integer, ByVal region As Integer) As List(Of inventaro)
        Dim SQL As String = "SELECT a.Des_Art,b.Nom_Bod, a.cod_Art,a.costo_art, e.Existencia_Deta_Art - (SELECT isnull(sum(d.cantidad_articulo),0) " &
            "FROM  [DETA_RESERVA] d WHERE d.Id_Bod = b.Id_Bod and d.id_Art =  a.id_art and estado = 1) as existencia, " &
            "(select isnull(sum(cantidad),0) from  DET_TOMA_INVENTARIO d inner join  ENC_TOMA_INVENTARIO e on e.id_enc = d.id_enc WHERE e.estado = 1 and d.id_bod = b.Id_Bod and d.id_art = a.id_art and d.estado = 1 )  as tomadeinventario " &
            "FROM  Existencias e " &
            "INNER JOIN  Bodegas b ON b.Id_Bod = e.Id_Bod " &
            "INNER JOIN  Articulo a On a.id_art = e.Id_Art " &
            "INNER JOIN  SUCURSALES s On s.id_suc = b.Id_suc " &
            "INNER JOIN  REGIONES r On r.id_region = s.id_region "



        If region > 0 Then
            SQL = SQL & " WHERE  r.id_region = " & region

            If sucursal > 0 Then
                SQL = SQL & " and s.id_suc = " & sucursal

                If bodega > 0 Then
                    SQL = SQL & " and b.Id_Bod = " & sucursal
                End If

            End If
        End If

        Dim result As List(Of [inventaro]) = New List(Of inventaro)()
        Dim TablaEncabezado As DataTable = manipular.ObtenerDatos(SQL)

        For i = 0 To TablaEncabezado.Rows.Count - 1
            For ii = 0 To 1
                Dim Elemento As New inventaro
                Elemento.producto = TablaEncabezado.Rows(i).Item("Des_Art")
                Elemento.codigo = TablaEncabezado.Rows(i).Item("cod_Art")
                Elemento.costo = Format(TablaEncabezado.Rows(i).Item("costo_art"), "##,##0.00")
                Elemento.Bodega = TablaEncabezado.Rows(i).Item("Nom_Bod")
                Elemento.existencia = TablaEncabezado.Rows(i).Item("existencia")
                Elemento.tomadeinventarios = TablaEncabezado.Rows(i).Item("tomadeinventario")
                Elemento.diferencia = Elemento.existencia - Elemento.tomadeinventarios
                result.Add(Elemento)
                ii = ii + 1
            Next
        Next

        Return result

    End Function



    'Metodo para Guardar Los datos
    <WebMethod()>
    Public Function Insertar(ByVal observacion As String, ByVal usuario As String) As String

        Dim sucursal As String = "SELECT id_sucursal  FROM  [USUARIO] where USUARIO = '" & usuario & "'"
        'consulta sql
        Dim sql As String = "INSERT INTO [dbo].[ENC_TOMA_INVENTARIO]([observaciones],[fecha],[id_suc],[estado],[usuario]) VALUES('" & observacion & "',GETDATE(),(" & sucursal & "),1,'" & usuario & "');"


        Dim result As String = ""


        'ejecuta el query a travez de la clase manipular 
        If (manipular.EjecutaTransaccion1(sql)) Then
            result = "SUCCESS|Datos Insertador Correctamente."
        Else
            result = "ERROR|Sucedio Un error, Por Favor Comuníquese con el Administrador. "
        End If


        Return result
    End Function

    'Metodo para Guardar Los datos
    <WebMethod()>
    Public Function InsertarDetalle(ByVal observacion As String, ByVal usuario As String, ByVal bodega As Integer, ByVal producto As Integer, ByVal cantidad As Integer, ByVal idtoma As Integer) As String

        'consulta sql
        Dim sql As String = "INSERT INTO [dbo].[DET_TOMA_INVENTARIO]([id_enc],[id_art],[cantidad],[id_bod],[observacion],[fecha],[estado],[usuario])" &
            "VALUES(" & idtoma & ", " & producto & "," & cantidad & "," & bodega & ",'" & observacion & "',GETDATE(),1,'" & usuario & "')"


        Dim result As String = ""


        'ejecuta el query a travez de la clase manipular 
        If (manipular.EjecutaTransaccion1(sql)) Then
            result = "SUCCESS|Datos Insertador Correctamente."
        Else
            result = "ERROR|Sucedio Un error, Por Favor Comuníquese con el Administrador. "
        End If


        Return result
    End Function

    'Metodo para Guardar Los datos
    <WebMethod()>
    Public Function Cerar(ByVal id As Integer) As String

        'consulta sql
        Dim sql As String = "UPDATE [dbo].[ENC_TOMA_INVENTARIO] set estado  = 2  WHERE id_enc = " & id


        Dim result As String = ""


        'ejecuta el query a travez de la clase manipular 
        If (manipular.EjecutaTransaccion1(sql)) Then
            result = "SUCCESS|Datos Insertador Correctamente."
        Else
            result = "ERROR|Sucedio Un error, Por Favor Comuníquese con el Administrador. "
        End If


        Return result
    End Function


    Public Class datos
        Public id As Integer
        Public fecha As String
        Public hora As String
        Public usuario As String
        Public Sucursal As String
        Public idsucursal As String
        Public observaciones As String
    End Class



    Public Class inventaro
        Public producto As String
        Public codigo As String
        Public costo As String
        Public Bodega As String
        Public existencia As Integer
        Public tomadeinventarios As Integer
        Public diferencia As Integer
    End Class

End Class
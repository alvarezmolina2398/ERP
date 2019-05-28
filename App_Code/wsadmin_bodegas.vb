Imports System.Data
Imports System.Web
Imports System.Web.Services
Imports System.Web.Services.Protocols

' Para permitir que se llame a este servicio web desde un script, usando ASP.NET AJAX, quite la marca de comentario de la línea siguiente.
<System.Web.Script.Services.ScriptService()>
<WebService(Namespace:="http://tempuri.org/")> _
<WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Public Class wsadmin_bodegas
    Inherits System.Web.Services.WebService

    'accion para obtener las bodegas
    <WebMethod()>
    Public Function ObtenerDatos() As List(Of datos)
        Dim SQL As String = "SELECT b.Id_Bod, e.id_empresa, s.id_suc,b.Nom_Bod,b.Observ_Bod, e.nombre as empresa, s.descripcion as sucursal,r.id_region, r.descripcion as region, b.principal " &
            "FROM  [Bodegas] b " &
            "INNER JOIN  [ENCA_CIA] e ON e.id_empresa = b.Id_Empsa  " &
            "INNER JOIN  [SUCURSALES] s ON s.id_suc = b.Id_suc " &
            "INNER JOIN  [REGIONES] r ON r.id_region = s.id_region " &
            "WHERE b.estado = 1 and s.estado = 1 and e.estado = 1 and r.estado = 1 "

        Dim result As List(Of [datos]) = New List(Of datos)()
        Dim TablaEncabezado As DataTable = manipular.ObtenerDatos(SQL)

        For i = 0 To TablaEncabezado.Rows.Count - 1
            For ii = 0 To 1
                Dim Elemento As New datos
                Elemento.id = TablaEncabezado.Rows(i).Item("Id_Bod")
                Elemento.idempresa = TablaEncabezado.Rows(i).Item("id_empresa")
                Elemento.idsucursal = TablaEncabezado.Rows(i).Item("id_suc")
                Elemento.descripcion = TablaEncabezado.Rows(i).Item("Nom_Bod").ToString
                Elemento.observacion = TablaEncabezado.Rows(i).Item("Observ_Bod").ToString
                Elemento.empresa = TablaEncabezado.Rows(i).Item("empresa").ToString
                Elemento.sucursal = TablaEncabezado.Rows(i).Item("sucursal").ToString
                Elemento.idregion = TablaEncabezado.Rows(i).Item("id_region").ToString
                Elemento.region = TablaEncabezado.Rows(i).Item("region").ToString
                Elemento.prioridad = TablaEncabezado.Rows(i).Item("principal")
                result.Add(Elemento)
                ii = ii + 1
            Next
        Next

        Return result

    End Function



    <WebMethod()>
    Public Function ObtenerDatosPorSucursal(ByVal usuario As String) As List(Of datos)
        Dim SQL As String = "SELECT b.Id_Bod, e.id_empresa, s.id_suc,b.Nom_Bod,b.Observ_Bod, e.nombre as empresa, s.descripcion as sucursal,r.id_region, r.descripcion as region, b.principal " &
            "FROM  [Bodegas] b " &
            "INNER JOIN  [ENCA_CIA] e ON e.id_empresa = b.Id_Empsa  " &
            "INNER JOIN  [SUCURSALES] s ON s.id_suc = b.Id_suc " &
            "INNER JOIN  [REGIONES] r ON r.id_region = s.id_region " &
            "WHERE b.estado = 1 and s.estado = 1 and e.estado = 1 and r.estado = 1 and s.id_suc = (SELECT id_sucursal FROM  [USUARIO] WHERE USUARIO = '" & usuario & "') "

        Dim result As List(Of [datos]) = New List(Of datos)()
        Dim TablaEncabezado As DataTable = manipular.ObtenerDatos(SQL)

        For i = 0 To TablaEncabezado.Rows.Count - 1
            For ii = 0 To 1
                Dim Elemento As New datos
                Elemento.id = TablaEncabezado.Rows(i).Item("Id_Bod")
                Elemento.idempresa = TablaEncabezado.Rows(i).Item("id_empresa")
                Elemento.idsucursal = TablaEncabezado.Rows(i).Item("id_suc")
                Elemento.descripcion = TablaEncabezado.Rows(i).Item("Nom_Bod").ToString
                Elemento.observacion = TablaEncabezado.Rows(i).Item("Observ_Bod").ToString
                Elemento.empresa = TablaEncabezado.Rows(i).Item("empresa").ToString
                Elemento.sucursal = TablaEncabezado.Rows(i).Item("sucursal").ToString
                Elemento.idregion = TablaEncabezado.Rows(i).Item("id_region").ToString
                Elemento.region = TablaEncabezado.Rows(i).Item("region").ToString
                Elemento.prioridad = TablaEncabezado.Rows(i).Item("principal")
                result.Add(Elemento)
                ii = ii + 1
            Next
        Next

        Return result

    End Function

    <WebMethod()>
    Public Function ObtenerDatosIDSucursal(ByVal sucursal As Integer) As List(Of datos)
        Dim SQL As String = "SELECT b.Id_Bod, e.id_empresa, s.id_suc,b.Nom_Bod,b.Observ_Bod, e.nombre as empresa, s.descripcion as sucursal,r.id_region, r.descripcion as region, b.principal " &
            "FROM  [Bodegas] b " &
            "INNER JOIN  [ENCA_CIA] e ON e.id_empresa = b.Id_Empsa  " &
            "INNER JOIN  [SUCURSALES] s ON s.id_suc = b.Id_suc " &
            "INNER JOIN  [REGIONES] r ON r.id_region = s.id_region " &
            "WHERE b.estado = 1 and s.estado = 1 and e.estado = 1 and r.estado = 1 and s.id_suc = " & sucursal

        Dim result As List(Of [datos]) = New List(Of datos)()
        Dim TablaEncabezado As DataTable = manipular.ObtenerDatos(SQL)

        For i = 0 To TablaEncabezado.Rows.Count - 1
            For ii = 0 To 1
                Dim Elemento As New datos
                Elemento.id = TablaEncabezado.Rows(i).Item("Id_Bod")
                Elemento.idempresa = TablaEncabezado.Rows(i).Item("id_empresa")
                Elemento.idsucursal = TablaEncabezado.Rows(i).Item("id_suc")
                Elemento.descripcion = TablaEncabezado.Rows(i).Item("Nom_Bod").ToString
                Elemento.observacion = TablaEncabezado.Rows(i).Item("Observ_Bod").ToString
                Elemento.empresa = TablaEncabezado.Rows(i).Item("empresa").ToString
                Elemento.sucursal = TablaEncabezado.Rows(i).Item("sucursal").ToString
                Elemento.idregion = TablaEncabezado.Rows(i).Item("id_region").ToString
                Elemento.region = TablaEncabezado.Rows(i).Item("region").ToString
                Elemento.prioridad = TablaEncabezado.Rows(i).Item("principal")
                result.Add(Elemento)
                ii = ii + 1
            Next
        Next

        Return result

    End Function

    'Metodo para Guardar Los datos
    <WebMethod()>
    Public Function Insertar(ByVal descripcion As String, ByVal observacion As String, ByVal empresa As String, ByVal idsucursal As Integer, ByVal principal As Integer) As String
        'consulta sql
        Dim sql As String = "INSERT INTO  [Bodegas] (Id_suc,Id_Empsa,Nom_Bod,Observ_Bod,estado,principal) VALUES(" & idsucursal & ", " & empresa & ", '" & descripcion & "','" & observacion & "',1," & principal & ");"


        Dim result As String = ""


        'ejecuta el query a travez de la clase manipular 
        If (manipular.EjecutaTransaccion1(sql)) Then
            result = "SUCCESS|Datos Insertador Correctamente."
        Else
            result = "ERROR|Sucedio Un error, Por Favor Comuníquese con el Administrador. "
        End If


        Return result
    End Function



    'Metodo para Actualizar Los datos
    <WebMethod()>
    Public Function Actualizar(ByVal descripcion As String, ByVal observacion As String, ByVal empresa As String, ByVal idsucursal As Integer, ByVal id As Integer, ByVal principal As Integer) As String
        'consulta sql
        Dim sql As String = ""

        If principal = 1 Then
            sql = "UPDATE  [Bodegas] set principal = 0 where Id_Empsa=" & empresa & " and Id_suc = " & idsucursal & " and estado =1;"
        End If

        sql = sql & "UPDATE  [Bodegas] set  Id_suc = " & idsucursal & ",Id_Empsa = " & empresa & ",Nom_Bod = '" & descripcion & "',Observ_Bod = '" & observacion & "', principal = " & principal & " where Id_Bod = " & id

        Dim result As String = ""


        'ejecuta el query a travez de la clase manipular 
        If (manipular.EjecutaTransaccion1(sql)) Then
            result = "SUCCESS|Datos Actualizados Correctamente"
        Else
            result = "ERROR|Sucedio Un error, Por Favor Comuníquese con el Administrador. "
        End If


        Return result
    End Function


    'Metodo para Eliminar Los datos
    <WebMethod()>
    Public Function Inhabilitar(ByVal id As Integer) As String
        'consulta sql
        Dim sql As String = "UPDATE  [Bodegas] set   estado = 0 where Id_Bod = " & id


        Dim result As String = ""


        'ejecuta el query a travez de la clase manipular 
        If (manipular.EjecutaTransaccion1(sql)) Then
            result = "SUCCESS|Datos Actualizados Correctamente"
        Else
            result = "ERROR|Sucedio Un error, Por Favor Comuníquese con el Administrador. "
        End If


        Return result
    End Function

    <WebMethod()>
    Public Function ExistePrincipal(ByVal id As Integer) As Boolean
        Dim SQL As String = "Select  count(*) as cantidad from  [Bodegas] b WHERE estado = 1 And Id_Empsa = " & id & " And principal = 1"


        Dim resultado As Boolean = False
        Dim result As Integer = 0
        Dim TablaEncabezado As DataTable = manipular.ObtenerDatos(SQL)

        For i = 0 To TablaEncabezado.Rows.Count - 1
            result = TablaEncabezado.Rows(i).Item("cantidad")
        Next

        If result > 0 Then
            resultado = True
        Else
            resultado = False
        End If

        Return resultado

    End Function




    Public Class datos
        Public id As Integer
        Public descripcion As String
        Public observacion As String
        Public empresa As String
        Public sucursal As String
        Public idempresa As String
        Public idsucursal As String
        Public idregion As Integer
        Public region As String
        Public prioridad As Integer
    End Class


End Class
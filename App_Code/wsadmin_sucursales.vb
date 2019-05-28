Imports System.Data
Imports System.Web
Imports System.Web.Services
Imports System.Web.Services.Protocols

' Para permitir que se llame a este servicio web desde un script, usando ASP.NET AJAX, quite la marca de comentario de la línea siguiente.
<System.Web.Script.Services.ScriptService()>
<WebService(Namespace:="http://tempuri.org/")> _
<WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Public Class wsadmin_sucursales
    Inherits System.Web.Services.WebService


    'Metodo para cargar  los datos
    <WebMethod()>
    Public Function ObtenerDatos() As List(Of datos)
        Dim SQL As String = "SELECT s.id_suc, s.descripcion as sucursal, s.direccion,s.telefono,s.moneda, s.id_correlativo ,s.id_correlativoNota, s.id_correlativoRecibo,s.id_correlativoReciboCopia, s.id_correlativoDebito,s.id_correlativoCopia, s.id_correlativoAbono,  s.id_correlativoAbonoCopia, s.id_correlativoNotaCopia, s.id_correlativoDebitoCopia, r.id_region, r.descripcion as region, e.nombre as empresa, e.id_empresa, isnull(s.codigo,isnull(s.id_sucursal,0)) as codigo " &
                              " From  [SUCURSALES] s " &
                              " INNER JOIN  [REGIONES] r on r.id_region =  s.id_region " &
                              " INNER JOIN  [ENCA_CIA] e on e.id_empresa = s.id_empresa where e.estado = 1 and r.estado = 1 and s.estado = 1 "

        Dim result As List(Of [datos]) = New List(Of datos)()
        Dim TablaEncabezado As DataTable = manipular.ObtenerDatos(SQL)

        For i = 0 To TablaEncabezado.Rows.Count - 1
            For ii = 0 To 1
                Dim Elemento As New datos
                Elemento.id = TablaEncabezado.Rows(i).Item("id_suc")
                Elemento.descripcion = TablaEncabezado.Rows(i).Item("sucursal").ToString
                Elemento.direccion = TablaEncabezado.Rows(i).Item("direccion").ToString
                Elemento.telefono = TablaEncabezado.Rows(i).Item("telefono").ToString
                Elemento.idmoneda = TablaEncabezado.Rows(i).Item("moneda")
                Elemento.serieelec = TablaEncabezado.Rows(i).Item("id_correlativo")
                Elemento.serieNota = TablaEncabezado.Rows(i).Item("id_correlativoNota")
                Elemento.serieDebito = TablaEncabezado.Rows(i).Item("id_correlativoDebito")
                Elemento.serieCopia = TablaEncabezado.Rows(i).Item("id_correlativoCopia")
                Elemento.serieAbono = TablaEncabezado.Rows(i).Item("id_correlativoAbono")
                Elemento.serieNotaCopia = TablaEncabezado.Rows(i).Item("id_correlativoNotaCopia")
                Elemento.serieDebitoCopia = TablaEncabezado.Rows(i).Item("id_correlativoDebitoCopia")
                Elemento.serieAbonoCopia = TablaEncabezado.Rows(i).Item("id_correlativoAbonoCopia")
                Elemento.serieCaja = TablaEncabezado.Rows(i).Item("id_correlativoRecibo")
                Elemento.serieCajaCopia = TablaEncabezado.Rows(i).Item("id_correlativoReciboCopia")
                Elemento.idregion = TablaEncabezado.Rows(i).Item("id_region")
                Elemento.region = TablaEncabezado.Rows(i).Item("region").ToString
                Elemento.idcompania = TablaEncabezado.Rows(i).Item("id_empresa")
                Elemento.compania = TablaEncabezado.Rows(i).Item("empresa").ToString
                Elemento.codigo = TablaEncabezado.Rows(i).Item("codigo")
                result.Add(Elemento)
                ii = ii + 1
            Next
        Next

        Return result

    End Function

    'Metodo para cargar los datos por codigo
    <WebMethod()>
    Public Function ObtenerDatosPorID(ByVal idregion As Integer) As List(Of datos)
        Dim SQL As String = "SELECT s.id_suc, s.descripcion as sucursal, s.direccion,s.telefono,s.moneda, s.id_correlativo ,s.id_correlativoNota, s.id_correlativoRecibo,s.id_correlativoReciboCopia, s.id_correlativoDebito,s.id_correlativoCopia, s.id_correlativoAbono,  s.id_correlativoAbonoCopia, s.id_correlativoNotaCopia, s.id_correlativoDebitoCopia, r.id_region, r.descripcion as region, e.nombre as empresa, e.id_empresa, isnull(s.codigo,isnull(s.id_sucursal,0)) as codigo " &
                              " From  [SUCURSALES] s " &
                              " INNER JOIN  [REGIONES] r on r.id_region =  s.id_region " &
                              " INNER JOIN  [ENCA_CIA] e on e.id_empresa = s.id_empresa where e.estado = 1 and r.estado = 1 and s.estado = 1 and r.id_region = " & idregion

        Dim result As List(Of [datos]) = New List(Of datos)()
        Dim TablaEncabezado As DataTable = manipular.ObtenerDatos(SQL)

        For i = 0 To TablaEncabezado.Rows.Count - 1
            For ii = 0 To 1
                Dim Elemento As New datos
                Elemento.id = TablaEncabezado.Rows(i).Item("id_suc")
                Elemento.descripcion = TablaEncabezado.Rows(i).Item("sucursal").ToString
                Elemento.direccion = TablaEncabezado.Rows(i).Item("direccion").ToString
                Elemento.telefono = TablaEncabezado.Rows(i).Item("telefono").ToString
                Elemento.idmoneda = TablaEncabezado.Rows(i).Item("moneda")
                Elemento.serieelec = TablaEncabezado.Rows(i).Item("id_correlativo")
                Elemento.serieNota = TablaEncabezado.Rows(i).Item("id_correlativoNota")
                Elemento.serieDebito = TablaEncabezado.Rows(i).Item("id_correlativoDebito")
                Elemento.serieCopia = TablaEncabezado.Rows(i).Item("id_correlativoCopia")
                Elemento.serieAbono = TablaEncabezado.Rows(i).Item("id_correlativoAbono")
                Elemento.serieNotaCopia = TablaEncabezado.Rows(i).Item("id_correlativoNotaCopia")
                Elemento.serieDebitoCopia = TablaEncabezado.Rows(i).Item("id_correlativoDebitoCopia")
                Elemento.serieAbonoCopia = TablaEncabezado.Rows(i).Item("id_correlativoAbonoCopia")
                Elemento.serieCaja = TablaEncabezado.Rows(i).Item("id_correlativoRecibo")
                Elemento.serieCajaCopia = TablaEncabezado.Rows(i).Item("id_correlativoReciboCopia")
                Elemento.idregion = TablaEncabezado.Rows(i).Item("id_region")
                Elemento.region = TablaEncabezado.Rows(i).Item("region").ToString
                Elemento.idcompania = TablaEncabezado.Rows(i).Item("id_empresa")
                Elemento.compania = TablaEncabezado.Rows(i).Item("empresa").ToString
                Elemento.codigo = TablaEncabezado.Rows(i).Item("codigo")
                result.Add(Elemento)
                ii = ii + 1
            Next
        Next

        Return result

    End Function


    'Metodo para cargar los datos por codigo
    <WebMethod()>
    Public Function ObtenerDatosPorIDEmpresa(ByVal id As Integer) As List(Of datos)
        Dim SQL As String = "SELECT s.id_suc, s.descripcion as sucursal, s.direccion,s.telefono,s.moneda, s.id_correlativo ,s.id_correlativoNota, s.id_correlativoRecibo,s.id_correlativoReciboCopia, s.id_correlativoDebito,s.id_correlativoCopia, s.id_correlativoAbono,  s.id_correlativoAbonoCopia, s.id_correlativoNotaCopia, s.id_correlativoDebitoCopia, r.id_region, r.descripcion as region, e.nombre as empresa, e.id_empresa, isnull(s.codigo,isnull(s.id_sucursal,0)) as codigo " &
                              " From  [SUCURSALES] s " &
                              " INNER JOIN  [REGIONES] r on r.id_region =  s.id_region " &
                              " INNER JOIN  [ENCA_CIA] e on e.id_empresa = s.id_empresa where e.estado = 1 and r.estado = 1 and s.estado = 1 and e.id_empresa = " & id

        Dim result As List(Of [datos]) = New List(Of datos)()
        Dim TablaEncabezado As DataTable = manipular.ObtenerDatos(SQL)

        For i = 0 To TablaEncabezado.Rows.Count - 1
            For ii = 0 To 1
                Dim Elemento As New datos
                Elemento.id = TablaEncabezado.Rows(i).Item("id_suc")
                Elemento.descripcion = TablaEncabezado.Rows(i).Item("sucursal").ToString
                Elemento.direccion = TablaEncabezado.Rows(i).Item("direccion").ToString
                Elemento.telefono = TablaEncabezado.Rows(i).Item("telefono").ToString
                Elemento.idmoneda = TablaEncabezado.Rows(i).Item("moneda")
                Elemento.serieelec = TablaEncabezado.Rows(i).Item("id_correlativo")
                Elemento.serieNota = TablaEncabezado.Rows(i).Item("id_correlativoNota")
                Elemento.serieDebito = TablaEncabezado.Rows(i).Item("id_correlativoDebito")
                Elemento.serieCopia = TablaEncabezado.Rows(i).Item("id_correlativoCopia")
                Elemento.serieAbono = TablaEncabezado.Rows(i).Item("id_correlativoAbono")
                Elemento.serieNotaCopia = TablaEncabezado.Rows(i).Item("id_correlativoNotaCopia")
                Elemento.serieDebitoCopia = TablaEncabezado.Rows(i).Item("id_correlativoDebitoCopia")
                Elemento.serieAbonoCopia = TablaEncabezado.Rows(i).Item("id_correlativoAbonoCopia")
                Elemento.serieCaja = TablaEncabezado.Rows(i).Item("id_correlativoRecibo")
                Elemento.serieCajaCopia = TablaEncabezado.Rows(i).Item("id_correlativoReciboCopia")
                Elemento.idregion = TablaEncabezado.Rows(i).Item("id_region")
                Elemento.region = TablaEncabezado.Rows(i).Item("region").ToString
                Elemento.idcompania = TablaEncabezado.Rows(i).Item("id_empresa")
                Elemento.compania = TablaEncabezado.Rows(i).Item("empresa").ToString
                Elemento.codigo = TablaEncabezado.Rows(i).Item("codigo")
                result.Add(Elemento)
                ii = ii + 1
            Next
        Next

        Return result

    End Function

    'Metodo para Guardar Los datos
    <WebMethod()>
    Public Function Insertar(ByVal id_empresa As Integer, ByVal id_region As Integer, ByVal codigo As String, ByVal descripcion As String, ByVal id_correlativo As Integer,
                             ByVal id_correlativoNota As Integer, ByVal id_correlativoDebito As Integer, ByVal moneda As Integer, ByVal direccion As String, ByVal id_correlativoCopia As Integer, ByVal id_correlativoAbono As Integer,
                             ByVal telefono As String, ByVal id_correlativoAbonoCopia As Integer, ByVal id_correlativoNotaCopia As Integer, ByVal id_correlativoDebitoCopia As Integer, ByVal id_correlativoRecibo As Integer, ByVal id_correlativoReciboCopia As Integer) As String
        'consulta sql

        If (codigo = "") Then
            codigo = "(SELECT ISNULL(MAX(codigo),0)+1 FROM   [SUCURSALES] WHERE id_empresa = " & id_empresa & " and id_region = " & id_region & ")"
        End If
        Dim sql As String = "INSERT INTO   [SUCURSALES] ([id_empresa],[id_region],[id_sucursal],[estado],[descripcion],[id_correlativo],[id_correlativoNota],[id_correlativoDebito],[moneda],[direccion],[codigo],[id_correlativoCopia],[telefono],[id_correlativoAbono],[id_correlativoAbonoCopia],[id_correlativoNotaCopia],[id_correlativoDebitoCopia],[id_correlativoRecibo],[id_correlativoReciboCopia]) " &
       "VALUES (" & id_empresa & ", " & id_region & ", " & codigo & ",1, '" & descripcion & "', " & id_correlativo & ", " & id_correlativoNota & ", " & id_correlativoDebito & "," & moneda & ",'" & direccion & "'," & codigo & ", " & id_correlativoCopia & ", '" & telefono & "', " & id_correlativoAbono & ", " & id_correlativoAbonoCopia & "," & id_correlativoNotaCopia & "," & id_correlativoDebitoCopia & ", " & id_correlativoRecibo & ", " & id_correlativoReciboCopia & ");"



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
    Public Function Actualizar(ByVal id_empresa As Integer, ByVal id_region As Integer, ByVal codigo As String, ByVal descripcion As String, ByVal id_correlativo As Integer,
                             ByVal id_correlativoNota As Integer, ByVal id_correlativoDebito As Integer, ByVal moneda As Integer, ByVal direccion As String, ByVal id_correlativoCopia As Integer, ByVal id_correlativoAbono As Integer,
                             ByVal telefono As String, ByVal id_correlativoAbonoCopia As Integer, ByVal id_correlativoNotaCopia As Integer, ByVal id_correlativoDebitoCopia As Integer, ByVal id_correlativoRecibo As Integer, ByVal id_correlativoReciboCopia As Integer, ByVal id As Integer) As String
        'consulta sql
        Dim sql As String = "UPDATE   [SUCURSALES] SET [id_empresa] = " & id_empresa &
               ",[id_region] = " & id_region &
               ",[id_sucursal] = " & codigo &
               ",[descripcion] = '" & descripcion & "' " &
               ",[id_correlativo] =  " & id_correlativo &
               ",[id_correlativoNota] = " & id_correlativoNota &
               ",[id_correlativoDebito] = " & id_correlativoDebito &
               ",[moneda] = " & moneda &
               ",[direccion] ='" & direccion & "' " &
               ",[codigo]  =  " & codigo &
               ",[id_correlativoCopia] = " & id_correlativoCopia &
               ",[telefono] = '" & telefono & "' " &
               ",[id_correlativoAbono] = " & id_correlativoAbono &
               " ,[id_correlativoAbonoCopia] = " & id_correlativoAbonoCopia &
               ",[id_correlativoNotaCopia] = " & id_correlativoNotaCopia &
               ",[id_correlativoDebitoCopia] = " & id_correlativoDebitoCopia &
               ",[id_correlativoRecibo] = " & id_correlativoRecibo &
               ",[id_correlativoReciboCopia] = " & id_correlativoReciboCopia &
               " WHERE id_suc = " & id


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
        Dim sql As String = "UPDATE   [SUCURSALES] set   estado = 0 where id_suc = " & id


        Dim result As String = ""


        'ejecuta el query a travez de la clase manipular 
        If (manipular.EjecutaTransaccion1(sql)) Then
            result = "SUCCESS|Datos Actualizados Correctamente"
        Else
            result = "ERROR|Sucedio Un error, Por Favor Comuníquese con el Administrador. "
        End If


        Return result
    End Function

    Public Class datos
        Public id As Integer
        Public direccion As String
        Public codigo As String
        Public descripcion As String
        Public compania As String
        Public idcompania As String
        Public serieelec As Integer
        Public serieCopia As Integer
        Public serieNota As Integer
        Public serieDebito As Integer
        Public serieNotaCopia As Integer
        Public serieAbono As Integer
        Public idmoneda As Integer
        Public telefono As String
        Public region As String
        Public idregion As Integer
        Public serieDebitoCopia As Integer
        Public serieAbonoCopia As Integer
        Public serieCaja As Integer
        Public serieCajaCopia As Integer
    End Class

End Class
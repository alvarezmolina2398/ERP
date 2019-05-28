Imports System.Data
Imports System.Data.SqlClient
Imports System.Web
Imports System.Web.Services
Imports System.Web.Services.Protocols

' Para permitir que se llame a este servicio web desde un script, usando ASP.NET AJAX, quite la marca de comentario de la línea siguiente.
<System.Web.Script.Services.ScriptService()>
<WebService(Namespace:="http://tempuri.org/")> _
<WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Public Class wscombos
    Inherits System.Web.Services.WebService

    <WebMethod()>
    Public Function Guardar(ByVal usuario As String, ByVal combo As Integer, ByVal listproductos As List(Of productos), ByVal producir As Integer) As String


        Dim conexion As SqlConnection
        conexion = New SqlConnection()
        conexion.ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings("ConString").ConnectionString
        conexion.Open()
        Dim comando As New SqlCommand
        Dim transaccion As SqlTransaction
        transaccion = conexion.BeginTransaction
        comando.Connection = conexion
        comando.Transaction = transaccion


        Dim result As String = ""

        Try

            Dim sql As String = "INSERT INTO  [ENC_COMBO]([fecha],[usuario],[id_combo],[estado],[tipo],cantidad) VALUES(GETDATE(),'" & usuario & "'," & combo & ",1,1," & producir & ")"


            'ejecuto primer comando sql
            comando.CommandText = sql
            comando.ExecuteNonQuery()


            'OBTENEMOS ID DE LA FACTURA
            comando.CommandText = "SELECT @@IDENTITY"
            Dim id As Integer = comando.ExecuteScalar()


            Dim sqlproduccion As String = "INSERT INTO  [ENC_ORDEN_PRODUCION]([fecha],[usuario],[id_combo],[estado],[tipo],cantidad) VALUES(GETDATE(),'" & usuario & "'," & combo & ",1,1," & producir & ")"


            'ejecuto primer comando sql
            comando.CommandText = sqlproduccion
            comando.ExecuteNonQuery()


            'OBTENEMOS ID DE LA FACTURA
            comando.CommandText = "SELECT @@IDENTITY"
            Dim id_produccion As Integer = comando.ExecuteScalar()

            For Each item As productos In listproductos

                Dim SQL_COSTO = "SELECT  costo_art as costo FROM Articulo where id_art  = " & item.id
                comando.CommandText = SQL_COSTO
                Dim costoacual As Double = comando.ExecuteScalar()

                Dim cantidad As Integer = ObtenerCantidadProducto(item.id, item.bodega)
                Dim precio As Double = costoacual * cantidad
                Dim preciocompra As Double = item.cantidad * item.precio
                Dim costofinal As Double = (precio + preciocompra)
                Dim nuevaExistencia As Integer = (cantidad + item.cantidad)


                'INSERTA LOS DATOS 
                Dim sql2 As String = "INSERT INTO  [DET_COMBO]([id_enc],[id_art],[precio],[cantidad],[estado]) VALUES(" & id & "," & item.id & "," & item.precio & "," & item.cantidad & ",1);INSERT INTO  [DET_ORDEN_PRODUCCION]([id_enc],[id_art],[precio],[cantidad],[estado]) VALUES(" & id_produccion & "," & item.id & "," & item.precio & "," & item.cantidad & ",1)"


                Dim sql3 As String = ""
                sql3 = "UPDATE  [Existencias] SET Existencia_Deta_Art =  " & cantidad - (item.cantidad * producir) & " WHERE [Id_Bod] = " & item.bodega & " and   Id_Art = " & item.id & "; " &
                    "UPDATE  [Articulo] SET [costo_art] = " & Math.Round((costofinal / nuevaExistencia), 2) & " WHERE id_art = " & item.id


                'ejecuto segundo comando sql
                comando.CommandText = sql2
                comando.ExecuteNonQuery()


                'ejecuto tercero comando sql
                comando.CommandText = sql3
                comando.ExecuteNonQuery()

            Next

            Dim bodega As Integer = ObtenerBodega(usuario)
            Dim cantidadCombo As Integer = ObtenerCantidadProducto(combo, bodega)

            Dim sql4 As String = ""
            If cantidadCombo < 0 Then
                sql4 = "INSERT INTO  [Existencias] ([Id_Bod],[Id_Art],[Existencia_Deta_Art]) VALUES(" & bodega & "," & combo & "," & producir & ")"
            Else
                sql4 = "UPDATE  [Existencias] SET Existencia_Deta_Art =  " & cantidadCombo + producir & " WHERE [Id_Bod] = " & bodega & " and   Id_Art = " & combo & "; "
            End If


            'ejecuto tercero comando sql
            comando.CommandText = sql4
            comando.ExecuteNonQuery()



            result = "SUCCESS|COMBO GENERADA EXITOSAMENTE"
            transaccion.Commit()

        Catch ex As Exception
            transaccion.Rollback()
            result = "ERROR|" & ex.Message
        Finally
            conexion.Close()
        End Try

        Return result
    End Function



    <WebMethod()>
    Public Function Actualizar(ByVal usuario As String, ByVal combo As Integer, ByVal listproductos As List(Of productos), ByVal producir As Integer, ByVal id As Integer) As String


        Dim conexion As SqlConnection
        conexion = New SqlConnection()
        conexion.ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings("ConString").ConnectionString
        conexion.Open()
        Dim comando As New SqlCommand
        Dim transaccion As SqlTransaction
        transaccion = conexion.BeginTransaction
        comando.Connection = conexion
        comando.Transaction = transaccion


        Dim result As String = ""

        Try

            Dim sql As String = "INSERT INTO [ENC_ORDEN_PRODUCION]([fecha],[usuario],[id_combo],[estado],[tipo],cantidad) VALUES(GETDATE(),'" & usuario & "'," & combo & ",1," & 1 & "," & producir & ")"

            comando.CommandText = sql
            comando.ExecuteNonQuery()


            'OBTENEMOS ID DE LA FACTURA
            comando.CommandText = "SELECT @@IDENTITY"
            Dim id_produccion As Integer = comando.ExecuteScalar()

            Dim r As Boolean = False
            For Each item As productos In listproductos

                Dim SQL_COSTO = "SELECT  costo_art as costo FROM Articulo where id_art  = " & item.id
                comando.CommandText = SQL_COSTO
                Dim costoacual As Double = comando.ExecuteScalar()

                Dim cantidad As Integer = ObtenerCantidadProducto(item.id, item.bodega)
                Dim precio As Double = costoacual * cantidad
                Dim preciocompra As Double = item.cantidad * item.precio
                Dim costofinal As Double = (precio + preciocompra)
                Dim nuevaExistencia As Integer = (cantidad + item.cantidad)


                If cantidad < item.cantidad * producir Then
                    result = "ERROR|NO EXISTE SUFICIENTE CANTIDAD DE UN DETERMINADO PRODUCTO"
                    r = False
                    Exit For
                Else
                    'INSERTA LOS DATOS 
                    Dim sql2 As String = "UPDATE  [DET_COMBO] SET ESTADO = 0 WHERE id_enc = " & id & " and id_art = " & item.id & ";INSERT INTO  [DET_COMBO]([id_enc],[id_art],[precio],[cantidad],[estado]) VALUES(" & id & "," & item.id & "," & item.precio & "," & item.cantidad & ",1);INSERT INTO  [DET_ORDEN_PRODUCCION]([id_enc],[id_art],[precio],[cantidad],[estado]) VALUES(" & id_produccion & "," & item.id & "," & item.precio & "," & item.cantidad & ",1)"

                    Dim sql3 As String = ""
                    sql3 = "UPDATE  [Existencias] SET Existencia_Deta_Art =  " & cantidad - (item.cantidad * producir) & " WHERE [Id_Bod] = " & item.bodega & " and   Id_Art = " & item.id & "; " &
                        "UPDATE  [Articulo] SET [costo_art] = " & Math.Round((costofinal / nuevaExistencia), 2) & " WHERE Id_Art = " & item.id

                    'ejecuto segundo comando sql
                    comando.CommandText = sql2
                    comando.ExecuteNonQuery()


                    'ejecuto tercero comando sql
                    comando.CommandText = sql3
                    comando.ExecuteNonQuery()
                    r = True

                End If


            Next


            If r Then


                Dim bodega As Integer = ObtenerBodega(usuario)
                Dim cantidadCombo As Integer = ObtenerCantidadProducto(combo, bodega)

                Dim sql4 As String = ""
                If cantidadCombo < 0 Then
                    sql4 = "INSERT INTO  [Existencias] ([Id_Bod],[Id_Art],[Existencia_Deta_Art]) VALUES(" & bodega & "," & combo & "," & producir & ")"
                Else
                    sql4 = "UPDATE  [Existencias] SET Existencia_Deta_Art =  " & cantidadCombo + producir & " WHERE [Id_Bod] = " & bodega & " and   Id_Art = " & combo & "; "
                End If


                'ejecuto tercero comando sql
                comando.CommandText = sql4
                comando.ExecuteNonQuery()



                result = "SUCCESS|COMBO GENERADA EXITOSAMENTE"
                transaccion.Commit()
            Else
                transaccion.Rollback()
            End If


        Catch ex As Exception
            transaccion.Rollback()
            result = "ERROR|" & ex.Message
        Finally
            conexion.Close()
        End Try

        Return result
    End Function


    <WebMethod()>
    Public Function ObtenerCantidadProducto(ByVal idart As Integer, ByVal bodega As Integer) As Integer
        Dim SQL As String = "Select Existencia_Deta_Art as cantidad from  Existencias where Id_Bod = " & bodega & " AND Id_Art = " & idart

        Dim result As Integer = -1
        Dim TablaEncabezado As DataTable = manipular.ObtenerDatos(SQL)

        For i = 0 To TablaEncabezado.Rows.Count - 1
            result = TablaEncabezado.Rows(i).Item("cantidad")
        Next

        Return result

    End Function

    <WebMethod()>
    Public Function ObtenerBodega(ByVal usuario As String) As Integer
        Dim SQL As String = "select Id_Bod from  Bodegas  where id_suc = (select id_sucursal from USUARIO where USUARIO = '" & usuario & "') and principal = 1 "

        Dim result As Integer = 0
        Dim TablaEncabezado As DataTable = manipular.ObtenerDatos(SQL)

        For i = 0 To TablaEncabezado.Rows.Count - 1
            result = TablaEncabezado.Rows(i).Item("Id_Bod")
        Next

        Return result

    End Function

    <WebMethod()>
    Public Function ObtenerCostoActual(ByVal idart As Integer) As Double
        Dim SQL As String = "SELECT e.Id_Art,  a.costo_art as costo FROM  [Existencias] e INNER JOIN   [Articulo]  a ON  a.id_art = e.Id_Art where a.id_art  = " & idart

        Dim result As Double = 0
        Dim TablaEncabezado As DataTable = manipular.ObtenerDatos(SQL)

        For i = 0 To TablaEncabezado.Rows.Count - 1

            result = TablaEncabezado.Rows(i).Item("costo")
        Next

        Return result

    End Function

    <WebMethod()>
    Public Function ProducirCombo(ByVal combo As Integer, ByVal usuario As String, ByVal producir As Integer, ByVal tipo As Integer) As String
        Dim conexion As SqlConnection
        conexion = New SqlConnection()
        conexion.ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings("ConString").ConnectionString
        conexion.Open()
        Dim comando As New SqlCommand
        Dim transaccion As SqlTransaction
        transaccion = conexion.BeginTransaction
        comando.Connection = conexion
        comando.Transaction = transaccion


        Dim result As String = ""

        Try

            Dim sql As String = "INSERT INTO [ENC_ORDEN_PRODUCION]([fecha],[usuario],[id_combo],[estado],[tipo],cantidad) VALUES(GETDATE(),'" & usuario & "'," & combo & ",1," & tipo & "," & producir & ")"


            'ejecuto primer comando sql
            comando.CommandText = sql
            comando.ExecuteNonQuery()



            'OBTENEMOS ID DE LA FACTURA
            comando.CommandText = "SELECT @@IDENTITY"
            Dim id As Integer = comando.ExecuteScalar()

            Dim listproductos As List(Of productos) = obtenerDetalleAnterior(combo)

            Dim r As Boolean = False

            For Each item As productos In listproductos

                'INSERTA LOS DATOS 
                Dim sql2 As String = "INSERT INTO [DET_ORDEN_PRODUCCION]([id_enc],[id_art],[precio],[cantidad],[estado]) VALUES(" & id & "," & item.id & "," & item.precio & "," & item.cantidad & ",1)"
                Dim bo As Integer = ObtenerBodega(usuario)
                Dim cantidad As Integer = ObtenerCantidadProducto(item.id, bo)


                If cantidad > (producir * item.cantidad) Then

                    Dim sql3 As String = ""
                    sql3 = "UPDATE  [Existencias] SET Existencia_Deta_Art =  " & cantidad - (item.cantidad * producir) & " WHERE [Id_Bod] = " & bo & " and   Id_Art = " & item.id & "; "


                    'ejecuto segundo comando sql
                    comando.CommandText = sql2
                    comando.ExecuteNonQuery()


                    'ejecuto tercero comando sql
                    comando.CommandText = sql3
                    comando.ExecuteNonQuery()
                    r = True
                Else
                    r = False
                    result = "ERROR| HAY UN PRODUCTO QUE NO TIENE SUFICIENTE CANTIDAD PARA AGREGAR AL COMBO"
                    Exit For
                End If
            Next



            If r Then
                Dim bodega As Integer = ObtenerBodega(usuario)
                Dim cantidadCombo As Integer = ObtenerCantidadProducto(combo, bodega)

                Dim sql4 As String = ""
                If cantidadCombo < 0 Then
                    sql4 = "INSERT INTO  [Existencias] ([Id_Bod],[Id_Art],[Existencia_Deta_Art]) VALUES(" & bodega & "," & combo & "," & producir & ")"
                Else
                    sql4 = "UPDATE  [Existencias] SET Existencia_Deta_Art =  " & cantidadCombo + producir & " WHERE [Id_Bod] = " & bodega & " and   Id_Art = " & combo & "; "
                End If


                'ejecuto tercero comando sql
                comando.CommandText = sql4
                comando.ExecuteNonQuery()



                result = "SUCCESS|COMBO GENERADA EXITOSAMENTE"
                transaccion.Commit()
            Else
                transaccion.Rollback()
            End If




        Catch ex As Exception
            transaccion.Rollback()
            result = "ERROR|" & ex.Message
        Finally
            conexion.Close()
        End Try

        Return result

    End Function

    'accion para obtener los datos 
    <WebMethod()>
    Public Function obtenerDetalleAnterior(ByVal idcombo As Integer) As List(Of productos)
        Dim consulta As String = "SELECT d.cantidad, d.precio, e.id_enc, d.id_art, a.cod_Art, a.Des_Art, e.usuario, (select b.Nom_Bod from Bodegas b where Id_Bod = (select Id_Bod from  Bodegas  where id_suc = (select id_sucursal from USUARIO where USUARIO = E.usuario and principal = 1))) as bodega from ENC_COMBO E INNER JOIN DET_COMBO D ON D.id_enc = E.id_enc INNER JOIN Articulo a on a.id_art = d.id_art WHERE d.estado = 1 and E.id_combo =" & idcombo


        Dim result As List(Of productos) = New List(Of productos)
        Dim TablaEncabezado As DataTable = manipular.ObtenerDatos(consulta)

        For i = 0 To TablaEncabezado.Rows.Count - 1
            Dim Elemento As productos = New productos()
            Elemento.cantidad = TablaEncabezado.Rows(i).Item("cantidad")
            Elemento.precio = TablaEncabezado.Rows(i).Item("precio")
            Elemento.id = TablaEncabezado.Rows(i).Item("id_art")
            Elemento.idcombo = TablaEncabezado.Rows(i).Item("id_enc")
            Elemento.codigo = TablaEncabezado.Rows(i).Item("cod_Art")
            Elemento.descripcion = TablaEncabezado.Rows(i).Item("Des_Art")
            Elemento.bodega = ObtenerBodega(TablaEncabezado.Rows(i).Item("usuario"))
            Elemento.bo = TablaEncabezado.Rows(i).Item("bodega")
            result.Add(Elemento)
        Next

        Return result


    End Function




    Public Class productos
        Public idcombo As Integer
        Public id As Integer
        Public cantidad As Integer
        Public bo As String
        Public bodega As Integer
        Public precio As Double
        Public descripcion As String
        Public codigo As String
    End Class

End Class
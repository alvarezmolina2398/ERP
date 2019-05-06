Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Web
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports iTextSharp.text
Imports iTextSharp.text.pdf

' Para permitir que se llame a este servicio web desde un script, usando ASP.NET AJAX, quite la marca de comentario de la línea siguiente.
<System.Web.Script.Services.ScriptService()>
<WebService(Namespace:="http://tempuri.org/")>
<WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Public Class wsorden_compra
    Inherits System.Web.Services.WebService

    <WebMethod()>
    Public Function Ordenar(ByVal proveedor As Integer, ByVal moneda As Integer, ByVal sucursal As Integer,
                            ByVal departamento As Integer, ByVal solicitante As Integer, ByVal tipoorden As Integer,
                            ByVal observacion As String, ByVal total As Double, ByVal usuario As String, ByVal listproductos As List(Of productos)) As String

        Dim result As String = ""
        Dim conexion As SqlConnection
        conexion = New SqlConnection()
        conexion.ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings("ConString").ConnectionString
        conexion.Open()
        Dim comando As New SqlCommand
        Dim transaccion As SqlTransaction
        transaccion = conexion.BeginTransaction
        comando.Connection = conexion
        comando.Transaction = transaccion



        Try
            Dim sql As String = "INSERT INTO [dbo].[ENC_OR_COMPRA]([Usuario],[id_empresa],[Fecha],[Total_Factura],[Id_Proveedor],[observaciones],[moneda],[estatus],[id_suc],[estado],[departamento],[empleado],[tipo],[moneda_local]) " &
            "VALUES ('" & usuario & "',(select top (1) id_empresa from USUARIO where USUARIO = '" & usuario & "'), getdate(), " & total & "," & proveedor & ", '" & observacion & "'," & moneda & ",1," & sucursal & ",1," & departamento & ", " & solicitante & "," & tipoorden & ",(SELECT TOP (1) [moneda] FROM [ERPDEVLYNGT].[dbo].[SUCURSALES] where id_suc = " & sucursal & "))"


            'ejecuto primer comando sql
            comando.CommandText = sql
            comando.ExecuteNonQuery()

            'OBTENEMOS ID DE LA FACTURA
            comando.CommandText = "SELECT @@IDENTITY"
            Dim id As Integer = comando.ExecuteScalar()

            For Each item As productos In listproductos

                'INSERTA LOS DATOS 
                Dim sql2 As String = "INSERT INTO [dbo].[DET_OR_COMPRA] ([id_enc],[Cantidad_Articulo],[Precio_Unit_Articulo],[Sub_Total],[Id_Art],[id_suc],[moneda_local]) " &
                    "VALUES (" & id & "," & item.cantidad & "," & item.precio & "," & (item.precio * item.cantidad) & "," & item.id & ", " & sucursal & ",(SELECT TOP (1) [moneda] FROM [ERPDEVLYNGT].[dbo].[SUCURSALES] where id_suc = " & sucursal & ")) "


                comando.CommandText = sql2
                comando.ExecuteNonQuery()

            Next
            transaccion.Commit()
            result = "SUCCESS|ORDEN DE COMPRA GENERADA EXITOSAMENTE|" & CrearPDF(id, sucursal, usuario, proveedor, observacion)
        Catch ex As Exception
            transaccion.Rollback()
            result = "ERROR|" & ex.Message
        Finally
            conexion.Close()
        End Try


        Return result
    End Function

    'compras en el interior
    <WebMethod()>
    Public Function Comprar(ByVal proveedor As Integer, ByVal moneda As Integer, ByVal sucursal As Integer,
                            ByVal departamento As Integer, ByVal solicitante As Integer, ByVal tipoorden As Integer,
                            ByVal observacion As String, ByVal total As Double, ByVal usuario As String, ByVal listproductos As List(Of productos), ByVal factura As String, ByVal serie As String, ByVal orden As String) As String



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

            If Not orden = "" Then
                Dim sql_update_orden As String = "UPDATE [dbo].[ENC_OR_COMPRA] SET  [estatus] = 0 WHERE id_enc  = " & orden
                comando.CommandText = sql_update_orden
                comando.ExecuteNonQuery()
            Else
                orden = "0"
            End If



            Dim sql As String = "INSERT INTO [dbo].[enc_compra_exterior]([usuario],[fecha],[facturas],[total_fact],[moneda_local],[idsuc],[iddeplab],[idsolicitante],[moneda],[estado],[tipo],[observaciones],[facturan],[idproveedor],[id_enc_orden]) " &
            "VALUES ('" & usuario & "', getdate(), '" & serie & "'," & total & ", (SELECT TOP (1) [moneda] FROM [ERPDEVLYNGT].[dbo].[SUCURSALES] where id_suc = " & sucursal & ")," & sucursal & "," & departamento & "," & solicitante & "," & moneda & ", 1," & tipoorden & ",'" & observacion & "','" & factura & "'," & proveedor & "," & orden & ")"


            'ejecuto primer comando sql
            comando.CommandText = sql
            comando.ExecuteNonQuery()


            'OBTENEMOS ID DE LA FACTURA
            comando.CommandText = "SELECT @@IDENTITY"
            Dim id As Integer = comando.ExecuteScalar()

            For Each item As productos In listproductos

                'INSERTA LOS DATOS 
                Dim sql2 As String = "INSERT INTO [dbo].[DET_COMPRA_exterior]([id_enc_orcompra_ing],[id_art],[cantidad],[valor],[moneda_local],[subtotal],[idsuc]) " &
                    "VALUES (" & id & "," & item.id & "," & item.cantidad & "," & item.precio & ",(SELECT TOP (1) [moneda] FROM [ERPDEVLYNGT].[dbo].[SUCURSALES] where id_suc = " & sucursal & ")," & (item.cantidad * item.precio) & "," & sucursal & ") "

                Dim cantidad As Integer = ObtenerCantidadProducto(item.id)

                Dim sql3 = ""
                Dim sqlbodega = "(select Id_Bod from dbo.Bodegas  where id_suc =  " & sucursal & " and principal = 1)"
                If cantidad < 0 Then

                    sql3 = "INSERT INTO [dbo].[Existencias]([Id_Bod],[Id_Art],[Existencia_Deta_Art]) VALUES(" & sqlbodega & ", " & item.id & "," & item.cantidad & "); " &
                    "UPDATE [dbo].[Articulo] SET [costo_art] = " & item.precio & " WHERE id_art = " & item.id
                Else
                    Dim precio As Double = ObtenerCostoActual(item.id) * cantidad
                    Dim preciocompra As Double = item.cantidad * item.precio
                    Dim costofinal As Double = (precio + preciocompra)
                    Dim nuevaExistencia As Integer = (cantidad + item.cantidad)

                    sql3 = "UPDATE [dbo].[Existencias] SET Existencia_Deta_Art =  " & nuevaExistencia & " WHERE [Id_Bod] = " & sqlbodega & " and   Id_Art = " & item.id & "; " &
                    "UPDATE [dbo].[Articulo] SET [costo_art] = " & Math.Round((costofinal / nuevaExistencia), 2) & " WHERE id_art = " & item.id
                End If


                'ejecuto segundo comando sql
                comando.CommandText = sql2
                comando.ExecuteNonQuery()


                'ejecuto tercero comando sql
                comando.CommandText = sql3
                comando.ExecuteNonQuery()

            Next

            result = "SUCCESS|COMPRA GENERADA EXITOSAMENTE"
            transaccion.Commit()

        Catch ex As Exception
            transaccion.Rollback()
            result = "ERROR|" & ex.Message
        Finally
            conexion.Close()
        End Try

        Return result
    End Function

    'compras en el exterior
    <WebMethod()>
    Public Function ComprarExterior(ByVal proveedor As Integer, ByVal moneda As Integer, ByVal sucursal As Integer,
                            ByVal departamento As Integer, ByVal solicitante As Integer, ByVal tipoorden As Integer,
                            ByVal observacion As String, ByVal total As Double, ByVal usuario As String, ByVal listproductos As List(Of productos), ByVal factura As String, ByVal serie As String,
                            ByVal fletee As Double, ByVal seguroe As Double, ByVal otrosge As Double, ByVal creditoe As Double, ByVal iva As Double, ByVal fletel As Double, ByVal agentel As Double,
                            ByVal almacenajel As Double, ByVal arancelt As Double, ByVal tasac As Double, ByVal polizan As Integer, ByVal porcentaje As Double, ByVal fechaingreso As String, ByVal orden As String) As String

        Dim fechaformat As String() = fechaingreso.Split("/")

        fechaingreso = fechaformat(2) & "-" & fechaformat(1) & "-" & fechaformat(0)

        Dim result As String = ""


        Dim conexion As SqlConnection
        conexion = New SqlConnection()
        conexion.ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings("ConString").ConnectionString
        conexion.Open()


        Dim comando As New SqlCommand
        Dim transaccion As SqlTransaction
        transaccion = conexion.BeginTransaction
        comando.Connection = conexion
        comando.Transaction = transaccion



        Try
            If Not orden = "" Then
                Dim sql_update_orden As String = "UPDATE [dbo].[ENC_OR_COMPRA] SET  [estatus] = 0 WHERE id_enc  = " & orden
                comando.CommandText = sql_update_orden
                comando.ExecuteNonQuery()
            Else
                orden = "0"
            End If

            Dim sql As String = "INSERT INTO [dbo].[enc_compra_exterior]([usuario],[fecha],[facturas],[total_fact],[moneda_local],[idsuc],[iddeplab],[idsolicitante],[moneda],[estado],[tipo],[observaciones],[facturan],[idproveedor],[fletee],[seguroe],[otrosge],[creditoe],[iva],[fletel],[agentel],[almacenajel],[arancelt],[tasac],[polizan],[fecha_ingreso],[id_enc_orden]) " &
               "VALUES ('" & usuario & "', getdate(), '" & serie & "'," & total & ", (SELECT TOP (1) [moneda] FROM [ERPDEVLYNGT].[dbo].[SUCURSALES] where id_suc = " & sucursal & ")," & sucursal & "," & departamento & "," & solicitante & "," & moneda & ", 1," & tipoorden & ",'" & observacion & "','" & factura & "'," & proveedor & "," & fletee & "," & seguroe &
               ", " & otrosge & "," & creditoe & "," & iva & "," & fletel & "," & agentel & "," & almacenajel & "," & arancelt & "," & tasac & ",'" & polizan & "', '" & fechaingreso & "'," & orden & ")"

            'ejecuto primer comando sql
            comando.CommandText = sql
            comando.ExecuteNonQuery()


            'OBTENEMOS ID DE LA FACTURA
            comando.CommandText = "SELECT @@IDENTITY"
            Dim id As Integer = comando.ExecuteScalar()


            For Each item As productos In listproductos

                'INSERTA LOS DATOS 
                Dim sql2 As String = "INSERT INTO [dbo].[DET_COMPRA_exterior]([id_enc_orcompra_ing],[id_art],[cantidad],[valor],[moneda_local],[subtotal],[idsuc],[arancel]) " &
                "VALUES (" & id & "," & item.id & "," & item.cantidad & "," & item.precio & ",(SELECT TOP (1) [moneda] FROM [ERPDEVLYNGT].[dbo].[SUCURSALES] where id_suc = " & sucursal & ")," & (item.cantidad * item.precio) & "," & sucursal & "," & item.arancel & ") "

                Dim cantidad As Integer = ObtenerCantidadProducto(item.id)

                Dim sql3 = ""
                Dim sqlbodega = "(select Id_Bod from dbo.Bodegas  where id_suc =  " & sucursal & " and principal = 1)"
                If cantidad < 0 Then

                    sql3 = "INSERT INTO [dbo].[Existencias]([Id_Bod],[Id_Art],[Existencia_Deta_Art]) VALUES(" & sqlbodega & ", " & item.id & "," & item.cantidad & "); " &
                "UPDATE [dbo].[Articulo] SET [costo_art] = " & Math.Round((item.precio * (1 + porcentaje))) & " WHERE id_art = " & item.id
                Else
                    Dim precio As Double = ObtenerCostoActual(item.id)
                    Dim preciocompra As Double = (item.cantidad * item.precio) * (1 + porcentaje)
                    Dim costofinal As Double = (precio + preciocompra)
                    Dim nuevaExistencia As Integer = (cantidad + item.cantidad)

                    sql3 = "UPDATE [dbo].[Existencias] SET Existencia_Deta_Art =  " & nuevaExistencia & " WHERE [Id_Bod] = " & sqlbodega & " and   Id_Art = " & item.id & "; " &
                "UPDATE [dbo].[Articulo] SET [costo_art] = " & Math.Round((costofinal / nuevaExistencia), 2) & " WHERE id_art = " & item.id
                End If

                'ejecuto segundo comando sql
                comando.CommandText = sql2
                comando.ExecuteNonQuery()


                'ejecuto tercero comando sql
                comando.CommandText = sql3
                comando.ExecuteNonQuery()

            Next
            transaccion.Commit()
            result = "SUCCESS|COMPRA GENERADA EXITOSAMENTE"

        Catch ex As Exception
            'MsgBox(ex.Message.ToString)
            transaccion.Rollback()
            result = "ERROR|" & ex.Message
        Finally
            conexion.Close()
        End Try

        Return result
    End Function


    'Obtiene los datos de la orden
    <WebMethod()>
    Public Function obtenerDatosOrden(ByVal orden As Integer) As datosorden
        Dim result As datosorden = New datosorden()

        Dim SQL As String = "SELECT p.Id_PRO, p.nit_pro, p.Nom_pro, c.id_suc, c.moneda,c.observaciones, c.departamento, c.tipo, c.empleado FROM [ERPDEVLYNGT].[dbo].[ENC_OR_COMPRA] c INNER JOIN [ERPDEVLYNGT].[dbo].[PROVEEDOR] p  ON p.Id_PRO = c.Id_Proveedor where c.estatus = 1  and c.id_enc = " & orden

        Dim TablaEncabezado As DataTable = manipular.ObtenerDatos(SQL)
        Dim Elemento As New datosorden
        For i = 0 To TablaEncabezado.Rows.Count - 1
            For ii = 0 To 1

                Elemento.nit = TablaEncabezado.Rows(i).Item("nit_pro")
                Elemento.proveedor = TablaEncabezado.Rows(i).Item("Nom_pro").ToString
                Elemento.idproveedor = TablaEncabezado.Rows(i).Item("Id_PRO")
                Elemento.nit = TablaEncabezado.Rows(i).Item("nit_pro")
                Elemento.sucursal = TablaEncabezado.Rows(i).Item("id_suc")
                Elemento.moneda = TablaEncabezado.Rows(i).Item("moneda")
                Elemento.observaciones = TablaEncabezado.Rows(i).Item("observaciones").ToString
                Elemento.Departamento = TablaEncabezado.Rows(i).Item("departamento")
                Elemento.tipoorden = TablaEncabezado.Rows(i).Item("tipo")
                Elemento.empleado = TablaEncabezado.Rows(i).Item("empleado")
                Elemento.producto = obtenerProductos(orden)

                ii = ii + 1
            Next
        Next

        result = Elemento

        Return result

    End Function


    Public Function obtenerProductos(ByVal orden As Integer) As IList(Of productos)

        Dim SQL As String = "SELECT  c.Cantidad_Articulo,c.Precio_Unit_Articulo,c.Id_Art, a.cod_Art, a.Des_Art FROM [ERPDEVLYNGT].[dbo].[DET_OR_COMPRA] c INNER JOIN [ERPDEVLYNGT].[dbo].[Articulo] a ON a.id_art = c.Id_Art where  id_enc = " & orden

        Dim result As List(Of productos) = New List(Of productos)()
        Dim TablaEncabezado As DataTable = manipular.ObtenerDatos(SQL)
        For i = 0 To TablaEncabezado.Rows.Count - 1
            For ii = 0 To 1
                Dim Elemento As productos = New productos()
                Elemento.id = TablaEncabezado.Rows(i).Item("Id_Art")
                Elemento.cantidad = TablaEncabezado.Rows(i).Item("Cantidad_Articulo")
                Elemento.precio = TablaEncabezado.Rows(i).Item("Precio_Unit_Articulo")
                Elemento.descripcion = TablaEncabezado.Rows(i).Item("Des_Art")
                Elemento.codigo = TablaEncabezado.Rows(i).Item("cod_Art")
                Elemento.arancel = 0

                result.Add(Elemento)

                ii = ii + 1
            Next
        Next

        Return result
    End Function


    <WebMethod()>
    Public Function CrearPDF(ByVal orden As Integer, ByVal idsuc As Integer, ByVal usuario As String, ByVal idproveedor As Integer, ByVal observacion As String) As String
        Dim result As String = ""
        Try

            Dim doc As Document = New iTextSharp.text.Document(iTextSharp.text.PageSize.LETTER, 5, 5, 5, 5)
            Dim datafecha As Date = Now
            Dim nombredoc As String = "pdf\orden" & orden & ".pdf"
            Dim pd As PdfWriter = PdfWriter.GetInstance(doc, New FileStream(Server.MapPath("~\vista\" & nombredoc), FileMode.Create))
            result = nombredoc
            doc.AddTitle("ORDEN DE COMPRA " & orden)
            doc.AddAuthor("")
            doc.AddCreationDate()

            doc.Open()

            Dim PARRAFO_ESPACIO As Paragraph = New Paragraph(" ", FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.BOLD))
            PARRAFO_ESPACIO.Alignment = Element.ALIGN_CENTER

            Dim d As datos = obtenerDatosEmpresa(idsuc)

            doc.Add(PARRAFO_ESPACIO)
            doc.Add(PARRAFO_ESPACIO)
            doc.Add(PARRAFO_ESPACIO)



            Dim RutaImagen As String = Server.MapPath("~")
            Dim Imagen As Image = Image.GetInstance(RutaImagen & "\img\logo.png")

            Imagen.ScalePercent(30) 'escala al tamaño de la imagen
            Imagen.SetAbsolutePosition(25, 690)

            doc.Add(Imagen)

            Dim Tabla As PdfPTable = New PdfPTable(3)

            Tabla.TotalWidth = 550.0F
            Tabla.LockedWidth = True
            Tabla.SetWidths({33, 33, 34})

            Dim Celda As PdfPCell = New PdfPCell


            Celda = New PdfPCell(New Paragraph(" ", FontFactory.GetFont("Arial", 14, iTextSharp.text.Font.BOLD)))
            Celda.Colspan = 1
            Celda.BorderWidth = 0
            Celda.HorizontalAlignment = Element.ALIGN_LEFT
            Tabla.AddCell(Celda)

            Celda = New PdfPCell(New Paragraph(d.descripcionextra, FontFactory.GetFont("Arial", 14, iTextSharp.text.Font.BOLD)))
            Celda.BorderWidth = 0
            Celda.Colspan = 0
            Celda.HorizontalAlignment = Element.ALIGN_CENTER
            Tabla.AddCell(Celda)

            Celda = New PdfPCell(New Paragraph("Orden DE COMPRA", FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD)))
            Celda.BorderWidth = 1
            Celda.BorderWidthBottom = 0
            Celda.Colspan = 0
            Celda.HorizontalAlignment = Element.ALIGN_CENTER
            Tabla.AddCell(Celda)



            Celda = New PdfPCell(New Paragraph(" ", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)))
            Celda.BorderWidth = 0
            Celda.Colspan = 0
            Celda.HorizontalAlignment = Element.ALIGN_CENTER
            Tabla.AddCell(Celda)


            Celda = New PdfPCell(New Paragraph(d.descripcion, FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL)))
            Celda.BorderWidth = 0
            Celda.Colspan = 0
            Celda.HorizontalAlignment = Element.ALIGN_CENTER
            Tabla.AddCell(Celda)

            Celda = New PdfPCell(New Paragraph(orden, FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD)))
            Celda.BorderWidth = 1
            Celda.BorderWidthBottom = 0
            Celda.BorderWidthTop = 0
            Celda.Colspan = 0
            Celda.HorizontalAlignment = Element.ALIGN_CENTER
            Tabla.AddCell(Celda)

            Celda = New PdfPCell(New Paragraph(" ", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)))
            Celda.BorderWidth = 0
            Celda.Colspan = 0
            Celda.HorizontalAlignment = Element.ALIGN_CENTER
            Tabla.AddCell(Celda)


            Celda = New PdfPCell(New Paragraph(d.direccion, FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL)))
            Celda.BorderWidth = 0
            Celda.Colspan = 0
            Celda.HorizontalAlignment = Element.ALIGN_CENTER
            Tabla.AddCell(Celda)

            Celda = New PdfPCell(New Paragraph(Date.Now, FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD)))
            Celda.BorderWidth = 1
            Celda.BorderWidthBottom = 0
            Celda.BorderWidthTop = 0
            Celda.Colspan = 0
            Celda.HorizontalAlignment = Element.ALIGN_CENTER
            Tabla.AddCell(Celda)

            Celda = New PdfPCell(New Paragraph(" ", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)))
            Celda.BorderWidth = 0
            Celda.Colspan = 0
            Celda.HorizontalAlignment = Element.ALIGN_CENTER
            Tabla.AddCell(Celda)


            Celda = New PdfPCell(New Paragraph(d.nit, FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL)))
            Celda.BorderWidth = 0
            Celda.Colspan = 0
            Celda.HorizontalAlignment = Element.ALIGN_CENTER
            Tabla.AddCell(Celda)

            Celda = New PdfPCell(New Paragraph(usuario, FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD)))
            Celda.BorderWidth = 1
            Celda.BorderWidthBottom = 1
            Celda.BorderWidthTop = 0
            Celda.Colspan = 0
            Celda.HorizontalAlignment = Element.ALIGN_CENTER
            Tabla.AddCell(Celda)


            doc.Add(Tabla)
            doc.Add(PARRAFO_ESPACIO)


            Dim dp As datos = obtenerDatosProveedor(idproveedor)

            Dim TablaCliente As PdfPTable = New PdfPTable(2)

            TablaCliente.TotalWidth = 550.0F
            TablaCliente.LockedWidth = True
            TablaCliente.SetWidths({20, 80})

            Celda = New PdfPCell(New Paragraph("NIT: ", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)))
            Celda.BorderWidth = 1
            Celda.BorderWidthBottom = 0
            Celda.Colspan = 0
            Celda.HorizontalAlignment = Element.ALIGN_JUSTIFIED
            TablaCliente.AddCell(Celda)


            Celda = New PdfPCell(New Paragraph(dp.nit, FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.NORMAL)))
            Celda.BorderWidth = 1
            Celda.BorderWidthBottom = 0
            Celda.Colspan = 0
            Celda.HorizontalAlignment = Element.ALIGN_JUSTIFIED
            TablaCliente.AddCell(Celda)

            Celda = New PdfPCell(New Paragraph("NOMBRE: ", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)))
            Celda.BorderWidth = 1
            Celda.BorderWidthTop = 0
            Celda.BorderWidthBottom = 0
            Celda.Colspan = 0
            Celda.HorizontalAlignment = Element.ALIGN_JUSTIFIED
            TablaCliente.AddCell(Celda)


            Celda = New PdfPCell(New Paragraph(dp.descripcion, FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.NORMAL)))
            Celda.BorderWidth = 1
            Celda.BorderWidthTop = 0
            Celda.BorderWidthBottom = 0
            Celda.Colspan = 0
            Celda.HorizontalAlignment = Element.ALIGN_JUSTIFIED
            TablaCliente.AddCell(Celda)

            Celda = New PdfPCell(New Paragraph("DIRECCION: ", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)))
            Celda.BorderWidth = 1
            Celda.BorderWidthTop = 0
            Celda.Colspan = 0
            Celda.HorizontalAlignment = Element.ALIGN_JUSTIFIED
            TablaCliente.AddCell(Celda)


            Celda = New PdfPCell(New Paragraph(dp.direccion, FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.NORMAL)))
            Celda.BorderWidth = 1
            Celda.BorderWidthTop = 0
            Celda.Colspan = 0
            Celda.HorizontalAlignment = Element.ALIGN_JUSTIFIED
            TablaCliente.AddCell(Celda)



            'AGREGAMOS LA TABLA DE DATOS DEL PROVEEDOR
            doc.Add(TablaCliente)
            doc.Add(PARRAFO_ESPACIO)



            Dim TablaDatos As PdfPTable = New PdfPTable(5)

            TablaDatos.TotalWidth = 550.0F
            TablaDatos.LockedWidth = True
            TablaDatos.SetWidths({15, 40, 15, 15, 15})

            Celda = New PdfPCell(New Paragraph("CANTIDAD", FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD)))
            Celda.BorderWidth = 1
            Celda.BorderWidthTop = 1
            Celda.BorderWidthRight = 0
            Celda.Colspan = 0
            Celda.HorizontalAlignment = Element.ALIGN_CENTER
            Celda.BackgroundColor = New iTextSharp.text.BaseColor(195, 199, 200)
            TablaDatos.AddCell(Celda)

            Celda = New PdfPCell(New Paragraph("NOMBRE", FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD)))
            Celda.BorderWidth = 1
            Celda.BorderWidthTop = 1
            Celda.BorderWidthRight = 0
            Celda.Colspan = 0
            Celda.HorizontalAlignment = Element.ALIGN_CENTER
            Celda.BackgroundColor = New iTextSharp.text.BaseColor(195, 199, 200)
            TablaDatos.AddCell(Celda)

            Celda = New PdfPCell(New Paragraph("PRECIO UNITARIO", FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD)))
            Celda.BorderWidth = 1
            Celda.BorderWidthTop = 1
            Celda.BorderWidthRight = 0
            Celda.Colspan = 0
            Celda.HorizontalAlignment = Element.ALIGN_CENTER
            Celda.BackgroundColor = New iTextSharp.text.BaseColor(195, 199, 200)
            TablaDatos.AddCell(Celda)

            Celda = New PdfPCell(New Paragraph("SUB-TOTAL", FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD)))
            Celda.BorderWidth = 1
            Celda.BorderWidthTop = 1
            Celda.BorderWidthRight = 0
            Celda.Colspan = 0
            Celda.HorizontalAlignment = Element.ALIGN_CENTER
            Celda.BackgroundColor = New iTextSharp.text.BaseColor(195, 199, 200)
            TablaDatos.AddCell(Celda)

            Celda = New PdfPCell(New Paragraph("UPC PROVEEDOR", FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD)))
            Celda.BorderWidth = 1
            Celda.BorderWidthTop = 1
            Celda.Colspan = 0
            Celda.HorizontalAlignment = Element.ALIGN_CENTER
            Celda.BackgroundColor = New iTextSharp.text.BaseColor(195, 199, 200)
            TablaDatos.AddCell(Celda)

            Dim total As Double = 0


            Dim SQL As String = "  SELECT c.Cantidad_Articulo, a.cod_Art,a.Des_Art, c.Precio_Unit_Articulo,c.Sub_Total, ISNULL(a.cod_pro1,' -- ') ups FROM [ERPDEVLYNGT].[dbo].[DET_OR_COMPRA]  c  " &
                "INNER JOIN [ERPDEVLYNGT].[dbo].[Articulo] a on a.id_art = c.Id_Art WHERE id_enc =  " & orden

            Dim TablaEncabezado As DataTable = manipular.ObtenerDatos(SQL)
            Dim Elemento As New datos
            For i = 0 To TablaEncabezado.Rows.Count - 1
                For ii = 0 To 1

                    Celda = New PdfPCell(New Paragraph(Format(TablaEncabezado.Rows(i).Item("Cantidad_Articulo"), "##,###"), FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL)))
                    Celda.BorderWidth = 1
                    Celda.BorderWidthTop = 0
                    Celda.BorderWidthRight = 0
                    Celda.HorizontalAlignment = Element.ALIGN_CENTER
                    TablaDatos.AddCell(Celda)

                    Celda = New PdfPCell(New Paragraph(TablaEncabezado.Rows(i).Item("cod_Art") & " - " & TablaEncabezado.Rows(i).Item("Des_Art"), FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL)))
                    Celda.BorderWidth = 1
                    Celda.BorderWidthTop = 0
                    Celda.BorderWidthRight = 0
                    Celda.Colspan = 0
                    Celda.HorizontalAlignment = Element.ALIGN_CENTER
                    TablaDatos.AddCell(Celda)

                    Celda = New PdfPCell(New Paragraph(Format(TablaEncabezado.Rows(i).Item("Precio_Unit_Articulo"), "##,##0.00"), FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL)))
                    Celda.BorderWidth = 1
                    Celda.BorderWidthTop = 0
                    Celda.BorderWidthRight = 0
                    Celda.Colspan = 0
                    Celda.HorizontalAlignment = Element.ALIGN_CENTER
                    TablaDatos.AddCell(Celda)

                    Celda = New PdfPCell(New Paragraph(Format(TablaEncabezado.Rows(i).Item("Sub_Total"), "##,##0.00"), FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL)))
                    Celda.BorderWidth = 1
                    Celda.BorderWidthTop = 0
                    Celda.BorderWidthRight = 0
                    Celda.Colspan = 0
                    Celda.HorizontalAlignment = Element.ALIGN_CENTER
                    TablaDatos.AddCell(Celda)

                    Celda = New PdfPCell(New Paragraph(TablaEncabezado.Rows(i).Item("ups").ToString, FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL)))
                    Celda.BorderWidth = 1
                    Celda.BorderWidthTop = 0
                    Celda.BorderWidthRight = 1
                    Celda.Colspan = 0
                    Celda.HorizontalAlignment = Element.ALIGN_CENTER
                    TablaDatos.AddCell(Celda)

                    total += TablaEncabezado.Rows(i).Item("Sub_Total")

                    ii = ii + 1
                Next
            Next

            Celda = New PdfPCell(New Paragraph(" ", FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD)))
            Celda.BorderWidth = 1
            Celda.BorderWidthTop = 0
            Celda.BorderWidthRight = 0
            Celda.Colspan = 0
            Celda.HorizontalAlignment = Element.ALIGN_CENTER
            Celda.BackgroundColor = New iTextSharp.text.BaseColor(195, 199, 200)
            TablaDatos.AddCell(Celda)

            Celda = New PdfPCell(New Paragraph("TOTAL", FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD)))
            Celda.BorderWidth = 1
            Celda.BorderWidthTop = 0
            Celda.BorderWidthRight = 0
            Celda.Colspan = 0
            Celda.HorizontalAlignment = Element.ALIGN_CENTER
            Celda.BackgroundColor = New iTextSharp.text.BaseColor(195, 199, 200)
            TablaDatos.AddCell(Celda)

            Celda = New PdfPCell(New Paragraph(" ", FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD)))
            Celda.BorderWidth = 1
            Celda.BorderWidthTop = 0
            Celda.BorderWidthRight = 0
            Celda.Colspan = 0
            Celda.HorizontalAlignment = Element.ALIGN_CENTER
            Celda.BackgroundColor = New iTextSharp.text.BaseColor(195, 199, 200)
            TablaDatos.AddCell(Celda)

            Celda = New PdfPCell(New Paragraph(Format(total, "##,##0.00"), FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD)))
            Celda.BorderWidth = 1
            Celda.BorderWidthTop = 0
            Celda.BorderWidthRight = 0
            Celda.Colspan = 0
            Celda.HorizontalAlignment = Element.ALIGN_CENTER
            Celda.BackgroundColor = New iTextSharp.text.BaseColor(195, 199, 200)
            TablaDatos.AddCell(Celda)

            Celda = New PdfPCell(New Paragraph(" ", FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD)))
            Celda.BorderWidth = 1
            Celda.BorderWidthTop = 0
            Celda.Colspan = 0
            Celda.HorizontalAlignment = Element.ALIGN_CENTER
            Celda.BackgroundColor = New iTextSharp.text.BaseColor(195, 199, 200)
            TablaDatos.AddCell(Celda)


            doc.Add(TablaDatos)

            doc.Add(PARRAFO_ESPACIO)

            Dim PARRAFO1 As Paragraph = New Paragraph("OBSERVACIONES: ", FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.BOLD))
            PARRAFO1.Alignment = Element.ALIGN_LEFT
            PARRAFO1.SpacingAfter = 10.0F
            PARRAFO1.IndentationLeft = 25.0F
            PARRAFO1.IndentationRight = 25.0F
            doc.Add(PARRAFO1)

            Dim PARRAFO2 As Paragraph = New Paragraph(observacion, FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.NORMAL))
            PARRAFO2.Alignment = Element.ALIGN_JUSTIFIED
            PARRAFO2.SpacingAfter = 10.0F
            PARRAFO2.IndentationLeft = 25.0F
            PARRAFO2.IndentationRight = 25.0F
            doc.Add(PARRAFO2)



            Dim PARRAFOFIRMA As Paragraph = New Paragraph("__________________________________________________", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.NORMAL))
            PARRAFOFIRMA.Alignment = Element.ALIGN_CENTER
            PARRAFOFIRMA.SpacingBefore = 25.0F
            doc.Add(PARRAFOFIRMA)

            PARRAFOFIRMA = New Paragraph("Firma de Autorización", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.NORMAL))
            PARRAFOFIRMA.Alignment = Element.ALIGN_CENTER
            doc.Add(PARRAFOFIRMA)


            doc.Close()


        Catch ex As Exception
            result = ex.Message
        End Try

        Return result
    End Function

    <WebMethod()>
    Public Function ObtenerCantidadProducto(ByVal idart As Integer) As Integer
        Dim SQL As String = "Select Existencia_Deta_Art as cantidad from dbo.Existencias where Id_Bod = 1 AND Id_Art = " & idart

        Dim result As Integer = -1
        Dim TablaEncabezado As DataTable = manipular.ObtenerDatos(SQL)

        For i = 0 To TablaEncabezado.Rows.Count - 1
            For ii = 0 To 1

                result = TablaEncabezado.Rows(i).Item("cantidad")
            Next
        Next

        Return result

    End Function


    <WebMethod()>
    Public Function ObtenerCostoActual(ByVal idart As Integer) As Double
        Dim SQL As String = "SELECT e.Id_Art,  a.costo_art as costo FROM [ERPDEVLYNGT].[dbo].[Existencias] e INNER JOIN [ERPDEVLYNGT].[dbo].[Articulo]  a ON  a.id_art = e.Id_Art where a.id_art  = " & idart

        Dim result As Double = 0
        Dim TablaEncabezado As DataTable = manipular.ObtenerDatos(SQL)

        For i = 0 To TablaEncabezado.Rows.Count - 1
            For ii = 0 To 1

                result = TablaEncabezado.Rows(i).Item("costo")
            Next
        Next

        Return result

    End Function

    <WebMethod()>
    Public Function obtenerDatosEmpresa(ByVal idsuc As Integer) As datos
        Dim SQL As String = "SELECT  top 1 [id_empresa],[nombre],[nombre_comercial],[direccion],[nit]  FROM [ERPDEVLYNGT].[dbo].[ENCA_CIA] " &
            " where id_empresa = (SELECT  s.id_empresa  FROM [ERPDEVLYNGT].[dbo].[SUCURSALES]  s where s.id_suc = " & idsuc & ")"

        Dim result As datos = New datos()
        Dim TablaEncabezado As DataTable = manipular.ObtenerDatos(SQL)
        Dim Elemento As New datos
        For i = 0 To TablaEncabezado.Rows.Count - 1
            For ii = 0 To 1

                Elemento.id = TablaEncabezado.Rows(i).Item("id_empresa")
                Elemento.descripcion = TablaEncabezado.Rows(i).Item("nombre").ToString
                Elemento.descripcionextra = TablaEncabezado.Rows(i).Item("nombre_comercial").ToString
                Elemento.nit = TablaEncabezado.Rows(i).Item("nit").ToString
                Elemento.direccion = TablaEncabezado.Rows(i).Item("direccion").ToString
                ii = ii + 1
            Next
        Next

        Return Elemento

    End Function

    <WebMethod()>
    Public Function obtenerDatosProveedor(ByVal id As Integer) As datos
        Dim SQL As String = "SELECT nit_pro,id_pro, Nom_pro, Dire_pro FROM dbo.PROVEEDOR where id_pro = " & id

        Dim result As datos = New datos()
        Dim TablaEncabezado As DataTable = manipular.ObtenerDatos(SQL)
        Dim Elemento As New datos
        For i = 0 To TablaEncabezado.Rows.Count - 1
            For ii = 0 To 1

                Elemento.id = TablaEncabezado.Rows(i).Item("id_pro")
                Elemento.descripcion = TablaEncabezado.Rows(i).Item("Nom_pro").ToString
                Elemento.direccion = TablaEncabezado.Rows(i).Item("Dire_pro").ToString
                Elemento.nit = TablaEncabezado.Rows(i).Item("nit_pro").ToString
                ii = ii + 1
            Next
        Next

        Return Elemento

    End Function

    Public Class datos
        Public id As Integer
        Public descripcion As String
        Public descripcionextra As String
        Public direccion As String
        Public nit As String
    End Class

    Public Class productos
        Public id As Integer
        Public cantidad As Integer
        Public precio As Double
        Public codigo As String
        Public descripcion As String
        Public arancel As Integer
    End Class


    Public Class datosorden
        Public observaciones As String
        Public idproveedor As Integer
        Public proveedor As String
        Public nit As String
        Public moneda As Integer
        Public sucursal As Integer
        Public Departamento As Integer
        Public solicitante As Integer
        Public tipoorden As Integer
        Public empleado As Integer
        Public producto As IList(Of productos)
    End Class

End Class
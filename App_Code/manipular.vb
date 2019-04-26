Imports Microsoft.Win32
Imports System.Data.SqlClient
Imports System.IO
Imports System.Web
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.Diagnostics
Imports System.Data
Imports System

Public Class manipular
    Private cn As New SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("ConString").ConnectionString)
    Private Consulta As String
    Public idempresa As Integer

    Shared Function cadenaCon() As String
        Return System.Configuration.ConfigurationManager.ConnectionStrings("ConString").ConnectionString
    End Function

    Public Shared Function ObtenerDatos(ByVal strsql As String) As System.Data.DataTable
        Dim cn As New SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("ConString").ConnectionString)
        cn.Open()
        Dim dt As New DataTable
        Try
            Dim cmd As New SqlCommand(strsql, cn)
            Dim da As New SqlDataAdapter(cmd)
            da.Fill(dt)
        Catch ex As Exception
            MsgBox(ex.Message)
        Finally
            cn.Close()
        End Try
        Return dt
    End Function
    Public Shared Function EjecutaTransaccion_devolverid(ByVal str1 As String) As Integer
        Dim transaccion1_result As Integer
        'Dim conexion As OdbcConnection
        'Dim comando As OdbcCommand
        Dim conexion As SqlConnection
        conexion = New SqlConnection()
        conexion.ConnectionString = cadenaCon()
        conexion.Open()
        Dim comando As New SqlCommand
        Dim transaccion As SqlTransaction
        transaccion = conexion.BeginTransaction
        comando.Connection = conexion
        comando.Transaction = transaccion
        Dim id As Integer = 0
        Try
            'ejecuto primer comando sql
            comando.CommandText = str1
            comando.ExecuteNonQuery()

            comando.CommandText = "SELECT @@IDENTITY"
            id = comando.ExecuteScalar()
            transaccion.Commit()

            transaccion1_result = id
        Catch ex As Exception
            'MsgBox(ex.Message.ToString)
            transaccion.Rollback()
            transaccion1_result = 0
        Finally
            conexion.Close()
        End Try
        Return transaccion1_result
    End Function


    Public Shared Function Login(ByVal strsql As String) As System.Data.DataTable
        Dim cn As New SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("ConString").ConnectionString)
        cn.Open()
        Dim dt As New DataTable
        Try
            Dim cmd As New SqlCommand(strsql, cn)
            Dim da As New SqlDataAdapter(cmd)
            da.Fill(dt)
        Catch ex As Exception
            'MsgBox(ex.Message)
        Finally
            cn.Close()
        End Try
        Return dt
    End Function

    Public Shared Function Login2(ByVal strsql As String) As System.Data.DataTable
        Dim cn As New SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("ConStringConta").ConnectionString)
        cn.Open()
        Dim dt As New DataTable
        Try
            Dim cmd As New SqlCommand(strsql, cn)
            Dim da As New SqlDataAdapter(cmd)
            da.Fill(dt)
        Catch ex As Exception
            ' MsgBox(ex.Message)
        Finally
            cn.Close()
        End Try
        Return dt
    End Function
    Public Shared Function Llena_Drop(ByVal strsql As String) As System.Data.DataTable
        Dim cn As New SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("ConString").ConnectionString)
        cn.Open()
        Dim dt As New DataTable
        Try
            Dim cmd As New SqlCommand(strsql, cn)
            Dim da As New SqlDataAdapter(cmd)
            da.Fill(dt)
        Catch ex As Exception
            ' MsgBox(ex.Message)
        Finally
            cn.Close()
        End Try
        Return dt
    End Function

    Public Shared Function Verificacion(ByVal strsql As String) As Boolean
        Dim cn As New SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("ConString").ConnectionString)
        cn.Open()
        Try
            Dim comando As New SqlCommand
            Dim transaccion As SqlTransaction
            transaccion = cn.BeginTransaction
            comando.Connection = cn
            comando.Transaction = transaccion

            comando.CommandText = strsql
            Dim Resultado1 As String = Convert.ToString(comando.ExecuteScalar)
            If Resultado1 = "" Then
                Verificacion = False
            Else
                Verificacion = True
            End If
        Catch ex As Exception
            Verificacion = False
            ' MsgBox(ex.Message)
        Finally
            cn.Close()
        End Try
    End Function

    Public Shared Function Drop_VB(ByVal strsql As String) As System.Data.DataTable
        Dim cn As New SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("ConString").ConnectionString)
        cn.Open()
        Dim dt As New DataTable
        Try
            Dim cmd As New SqlCommand(strsql, cn)
            Dim da As New SqlDataAdapter(cmd)
            da.Fill(dt)
        Catch ex As Exception
            ' MsgBox(ex.Message)
        Finally
            cn.Close()
        End Try
        Return dt
    End Function


    Public Shared Function EjecutaTransaccion1(ByVal str1 As String) As Boolean
        'Dim conexion As OdbcConnection
        'Dim comando As OdbcCommand
        Dim conexion As SqlConnection
        conexion = New SqlConnection()
        conexion.ConnectionString = cadenaCon()
        conexion.Open()
        Dim comando As New SqlCommand
        Dim transaccion As SqlTransaction
        transaccion = conexion.BeginTransaction
        comando.Connection = conexion
        comando.Transaction = transaccion

        Try
            'ejecuto primer comando sql
            comando.CommandText = str1
            comando.ExecuteNonQuery()
            transaccion.Commit()
            EjecutaTransaccion1 = True
        Catch ex As Exception
            'MsgBox(ex.Message.ToString)
            transaccion.Rollback()
            EjecutaTransaccion1 = False
        Finally
            conexion.Close()
        End Try
    End Function



    Public Shared Function EjecutaTransaccion1empresa(ByVal str1 As String) As Boolean
        'Dim conexion As OdbcConnection
        'Dim comando As OdbcCommand
        Dim conexion As SqlConnection
        conexion = New SqlConnection()
        conexion.ConnectionString = cadenaCon()
        conexion.Open()
        Dim comando As New SqlCommand
        Dim transaccion As SqlTransaction
        transaccion = conexion.BeginTransaction
        comando.Connection = conexion
        comando.Transaction = transaccion

        Try
            'ejecuto primer comando sql
            comando.CommandText = str1
            comando.ExecuteNonQuery()
            transaccion.Commit()
            EjecutaTransaccion1empresa = True
        Catch ex As Exception
            'MsgBox(ex.Message.ToString)
            transaccion.Rollback()
            EjecutaTransaccion1empresa = False
        Finally
            conexion.Close()
        End Try
    End Function

    Public Shared Function idempresabusca(ByVal strsql As String) As String
        Dim cn As New SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("ConString").ConnectionString)
        cn.Open()
        Dim Resultado1 As String
        Try
            Dim comando As New SqlCommand
            Dim transaccion As SqlTransaction
            transaccion = cn.BeginTransaction
            comando.Connection = cn
            comando.Transaction = transaccion

            comando.CommandText = strsql
            Resultado1 = Convert.ToString(comando.ExecuteScalar)

        Catch ex As Exception
            ' MsgBox(ex.Message)
        Finally
            cn.Close()
        End Try
        Return Resultado1
    End Function

    Public Shared Function EjecutaTransaccion(ByVal str1 As String, ByVal str2 As String) As Boolean
        'Dim conexion As OdbcConnection
        '    'Dim comando As OdbcCommand
        Dim conexion As SqlConnection
        conexion = New SqlConnection()
        conexion.ConnectionString = cadenaCon()
        conexion.Open()
        Dim comando As New SqlCommand
        Dim transaccion As SqlTransaction
        transaccion = conexion.BeginTransaction
        comando.Connection = conexion
        comando.Transaction = transaccion

        Try
            'ejecuto primer comando sql
            comando.CommandText = str1
            comando.ExecuteNonQuery()
            'ejecuto segundo comando sql
            comando.CommandText = str2
            comando.ExecuteNonQuery()
            transaccion.Commit()
            EjecutaTransaccion = True
        Catch ex As Exception
            'MsgBox(ex.Message.ToString)
            transaccion.Rollback()
            EjecutaTransaccion = False
        Finally
            conexion.Close()
        End Try
    End Function

    Public Shared Function EjecutaTransaccion_5(ByVal str1 As String, ByVal str2 As String, ByVal str3 As String, ByVal str4 As String, ByVal str5 As String, ByVal str6 As String) As Boolean
        'Dim conexion As OdbcConnection
        '    'Dim comando As OdbcCommand
        Dim conexion As SqlConnection
        conexion = New SqlConnection()
        conexion.ConnectionString = cadenaCon()
        conexion.Open()
        Dim comando As New SqlCommand
        Dim transaccion As SqlTransaction
        transaccion = conexion.BeginTransaction
        comando.Connection = conexion
        comando.Transaction = transaccion

        Try
            'ejecuto primer comando sql
            comando.CommandText = str1
            comando.ExecuteNonQuery()
            'ejecuto segundo comando sql
            comando.CommandText = str2
            comando.ExecuteNonQuery()
            'ejecuto tercer comando sql
            comando.CommandText = str3
            comando.ExecuteNonQuery()
            'ejecuto cuarto comando sql
            comando.CommandText = str4
            comando.ExecuteNonQuery()
            'ejecuto quinto comando sql
            comando.CommandText = str5
            comando.ExecuteNonQuery()
            'ejecuto sexto comando sql
            comando.CommandText = str6
            comando.ExecuteNonQuery()
            transaccion.Commit()
            EjecutaTransaccion_5 = True
        Catch ex As Exception
            'MsgBox(ex.Message.ToString)
            transaccion.Rollback()
            EjecutaTransaccion_5 = False
        Finally
            conexion.Close()
        End Try
    End Function

    Public Shared Function EjecutaTransaccion3(ByVal str1 As String, ByVal str2 As String, ByVal Str3 As String) As Boolean
        'Dim conexion As OdbcConnection
        'Dim comando As OdbcCommand
        Dim conexion As SqlConnection
        conexion = New SqlConnection()
        conexion.ConnectionString = cadenaCon()
        conexion.Open()
        Dim comando As New SqlCommand
        Dim transaccion As SqlTransaction
        transaccion = conexion.BeginTransaction
        comando.Connection = conexion
        comando.Transaction = transaccion

        Try
            'ejecuto primer comando sql
            comando.CommandText = str1
            comando.ExecuteNonQuery()
            'ejecuto segundo comando sql
            comando.CommandText = str2
            comando.ExecuteNonQuery()
            'ejecuto tercer comando sql
            comando.CommandText = Str3
            comando.ExecuteNonQuery()
            transaccion.Commit()
            EjecutaTransaccion3 = True
        Catch ex As Exception
            'MsgBox(ex.Message.ToString)
            transaccion.Rollback()
            EjecutaTransaccion3 = False
        Finally
            conexion.Close()
        End Try
    End Function

    'Public Shared Function EjecutaTransaccion4(ByVal str1 As String, ByVal str2 As String, ByVal Str3 As String, ByRef str4 As String) As Boolean
    '    'Dim conexion As OdbcConnection
    '    'Dim comando As OdbcCommand
    '    Dim conexion As SqlConnection
    '    conexion = New SqlConnection()
    '    conexion.ConnectionString = cadenaCon()
    '    conexion.Open()
    '    Dim comando As New SqlCommand
    '    Dim transaccion As SqlTransaction
    '    transaccion = conexion.BeginTransaction
    '    comando.Connection = conexion
    '    comando.Transaction = transaccion

    '    Try
    '        'ejecuto primer comando sql
    '        comando.CommandText = str1
    '        comando.ExecuteNonQuery()
    '        'ejecuto segundo comando sql
    '        comando.CommandText = str2
    '        comando.ExecuteNonQuery()
    '        'ejecuto tercer comando sql
    '        comando.CommandText = Str3
    '        comando.ExecuteNonQuery()
    '        'ejecuto cuarto comando sql
    '        comando.CommandText = str4
    '        comando.ExecuteNonQuery()
    '        transaccion.Commit()
    '        EjecutaTransaccion4 = True
    '    Catch ex As Exception
    '        'MsgBox(ex.Message.ToString)
    '        transaccion.Rollback()
    '        EjecutaTransaccion4 = False
    '    Finally
    '        conexion.Close()
    '    End Try
    'End Function

    'Public Shared Function EjecutaTransaccion5(ByVal str1 As String, ByVal str2 As String, ByVal Str3 As String, ByVal str4 As String, ByVal str5 As String) As Boolean
    '    'Dim conexion As OdbcConnection
    '    'Dim comando As OdbcCommand
    '    Dim conexion As SqlConnection
    '    conexion = New SqlConnection()
    '    conexion.ConnectionString = cadenaCon()
    '    conexion.Open()
    '    Dim comando As New SqlCommand
    '    Dim transaccion As SqlTransaction
    '    transaccion = conexion.BeginTransaction
    '    comando.Connection = conexion
    '    comando.Transaction = transaccion

    '    Try
    '        'ejecuto primer comando sql
    '        comando.CommandText = str1
    '        comando.ExecuteNonQuery()
    '        'ejecuto segundo comando sql
    '        comando.CommandText = str2
    '        comando.ExecuteNonQuery()
    '        'ejecuto tercer comando sql
    '        comando.CommandText = Str3
    '        comando.ExecuteNonQuery()
    '        'ejecuto cuarto comando sql
    '        comando.CommandText = str4
    '        comando.ExecuteNonQuery()
    '        'ejecuto quinto comando sql
    '        comando.CommandText = str5
    '        comando.ExecuteNonQuery()
    '        transaccion.Commit()
    '        EjecutaTransaccion5 = True
    '    Catch ex As Exception
    '        'MsgBox(ex.Message.ToString)
    '        transaccion.Rollback()
    '        EjecutaTransaccion5 = False
    '    Finally
    '        conexion.Close()
    '    End Try
    'End Function

    'Public Shared Function EjecutaTransaccion6(ByVal str1 As String, ByVal str2 As String, ByVal Str3 As String, ByVal str4 As String, ByVal str5 As String, ByVal str6 As String) As Boolean
    '    'Dim conexion As OdbcConnection
    '    'Dim comando As OdbcCommand
    '    Dim conexion As SqlConnection
    '    conexion = New SqlConnection()
    '    conexion.ConnectionString = cadenaCon()
    '    conexion.Open()
    '    Dim comando As New SqlCommand
    '    Dim transaccion As SqlTransaction
    '    transaccion = conexion.BeginTransaction
    '    comando.Connection = conexion
    '    comando.Transaction = transaccion

    '    Try
    '        'ejecuto primer comando sql
    '        comando.CommandText = str1
    '        comando.ExecuteNonQuery()
    '        'ejecuto segundo comando sql
    '        comando.CommandText = str2
    '        comando.ExecuteNonQuery()
    '        'ejecuto tercer comando sql
    '        comando.CommandText = Str3
    '        comando.ExecuteNonQuery()
    '        'ejecuto cuarto comando sql
    '        comando.CommandText = str4
    '        comando.ExecuteNonQuery()
    '        'ejecuto quinto comando sql
    '        comando.CommandText = str5
    '        comando.ExecuteNonQuery()
    '        'ejecuto sexto comando sql
    '        comando.CommandText = str6
    '        comando.ExecuteNonQuery()
    '        transaccion.Commit()
    '        EjecutaTransaccion6 = True
    '    Catch ex As Exception
    '        'MsgBox(ex.Message.ToString)
    '        transaccion.Rollback()
    '        EjecutaTransaccion6 = False
    '    Finally
    '        conexion.Close()
    '    End Try
    'End Function

    'Public Function Query(ByVal sql As String)
    '    Try
    '        Consulta = sql
    '    Catch ex As Exception
    '        'MsgBox(ex.Message)
    '    End Try
    '    Return 0
    'End Function

    'Public Function Ejecutar() As Boolean
    '    cn.Open()
    '    Try
    '        Dim cmd As New SqlCommand
    '        cmd.Connection = Me.cn
    '        cmd.CommandText = Consulta
    '        cmd.ExecuteNonQuery()
    '        cn.Close()
    '        Return True
    '    Catch ex As Exception
    '        'MsgBox(ex.Message)
    '        cn.Close()
    '        Return False
    '    End Try
    '    'Me.fin()
    'End Function

    'Public Function Logear(ByVal uss As String, ByVal pass As String)
    '    cn.Open()
    '    Dim cmd As New SqlCommand(Me.Consulta, Me.cn)
    '    Dim da As New SqlDataAdapter(cmd)
    '    Dim dt As New DataTable
    '    Try
    '        da.Fill(dt)
    '        If String.Compare(dt.Rows(0).Item("Nom_User"), uss, False) = 0 Then
    '            If String.Compare(dt.Rows(0).Item("Pass_User"), pass, False) = 0 Then
    '                Return dt
    '            Else
    '                dt = New DataTable
    '                Return dt
    '            End If
    '        Else
    '            dt = New DataTable
    '            Return dt
    '        End If
    '    Catch ex As Exception
    '        dt = New DataTable
    '        Return dt
    '    Finally
    '        Me.fin()
    '    End Try
    '    dt = New DataTable
    '    Return dt
    'End Function
    'Public Sub cargar_Grid(ByVal Datagridview As GridView, ByVal strsql As String)
    '    cn.Open()
    '    Try
    '        'Dim comando As New OdbcCommand(strsql, conexion)
    '        'Dim da As New OdbcDataAdapter(comando)
    '        Dim comando As New SqlCommand(strsql, cn)
    '        Dim da As New SqlDataAdapter(comando)

    '        Dim ds As New DataSet
    '        da.Fill(ds)
    '        Datagridview.DataSource = ds.Tables(0)
    '        Datagridview.DataBind()
    '        'Catch ex As OdbcException
    '    Catch ex As SqlException

    '        'MsgBox(ex)
    '    Finally
    '        Me.fin()
    '    End Try
    'End Sub
    'Public Function Permisos(ByVal cons As String)
    '    cn.Open()
    '    Dim cmd As New SqlCommand(cons, Me.cn)
    '    Try
    '        Dim da As New SqlDataAdapter(cmd)
    '        Dim dt As New DataTable
    '        da.Fill(dt)
    '        Return dt
    '    Catch ex As Exception
    '        'MsgBox(ex.Message)
    '    Finally
    '        Me.fin()
    '    End Try
    '    Return 0
    'End Function

    'Public Sub cargar_Combo(ByVal ComboBox As DropDownList, ByVal strsql As String)
    '    cn.Open()
    '    'Dim conexion As OdbcConnection
    '    Try
    '        'Dim comando As New OdbcCommand(strsql, conexion)
    '        'Dim da As New OdbcDataAdapter(comando)
    '        Dim comando As New SqlCommand(strsql, Me.cn)
    '        Dim da As New SqlDataAdapter(comando)
    '        Dim ds As New DataSet
    '        da.Fill(ds)
    '        ComboBox.DataSource = ds.Tables(0)
    '        ComboBox.DataValueField = ds.Tables(0).Columns(0).Caption.ToString
    '        'ComboBox.DisplayMember = ds.Tables(0).Columns(1).Caption.ToString
    '        ComboBox.DataTextField = ds.Tables(0).Columns(1).Caption.ToString
    '        'ComboBox.ValueMember = ds.Tables(0).Columns(0).Caption.ToString
    '        ComboBox.DataBind()
    '        ComboBox.SelectedIndex = -1
    '        ComboBox.Items.Insert(0, New ListItem("Seleccione Uno", "0"))
    '        'Catch ex As OdbcException
    '    Catch ex As Exception
    '        'MsgBox(ex.Message)
    '        'MessageBox.Show(ex.Message.ToString, _
    '        '              "error", MessageBoxButtons.OK, _
    '        '             MessageBoxIcon.Error)
    '    Finally
    '        Me.fin()
    '    End Try
    'End Sub

    'Public Sub cargar_Combo2(ByVal ComboBox As DropDownList, ByVal strsql As String, ByVal x As String, ByVal y As String, ByVal z As String)
    '    cn.Open()
    '    'Dim conexion As OdbcConnection
    '    Try
    '        'Dim comando As New OdbcCommand(strsql, conexion)
    '        'Dim da As New OdbcDataAdapter(comando)
    '        Dim comando As New SqlCommand(strsql, Me.cn)
    '        Dim da As New SqlDataAdapter(comando)
    '        Dim ds As New DataSet
    '        da.Fill(ds)
    '        ComboBox.DataSource = ds.Tables(0)
    '        ComboBox.DataValueField = ds.Tables(0).Columns(0).Caption.ToString
    '        'ComboBox.DisplayMember = ds.Tables(0).Columns(1).Caption.ToString
    '        ComboBox.DataTextField = ds.Tables(0).Columns(1).Caption.ToString
    '        'ComboBox.ValueMember = ds.Tables(0).Columns(0).Caption.ToString
    '        ComboBox.DataBind()
    '        ComboBox.SelectedIndex = 0
    '        comando = New SqlCommand(x, Me.cn)
    '        da = New SqlDataAdapter(comando)
    '        Dim dt As New DataTable
    '        da.Fill(dt)
    '        ComboBox.Items.Insert(0, New ListItem(dt.Rows(0).Item(y).ToString, dt.Rows(0).Item(z).ToString))
    '        'Catch ex As OdbcException
    '    Catch ex As Exception
    '        'MsgBox(ex.Message)
    '        'MessageBox.Show(ex.Message.ToString, _
    '        '              "error", MessageBoxButtons.OK, _
    '        '             MessageBoxIcon.Error)
    '    Finally
    '        Me.fin()
    '    End Try
    'End Sub

    'Public Sub cargar_Combo3(ByVal ComboBox As DropDownList, ByVal strsql As String)
    '    cn.Open()
    '    'Dim conexion As OdbcConnection
    '    Try
    '        'Dim comando As New OdbcCommand(strsql, conexion)
    '        'Dim da As New OdbcDataAdapter(comando)
    '        Dim comando As New SqlCommand(strsql, Me.cn)
    '        Dim da As New SqlDataAdapter(comando)
    '        Dim ds As New DataSet
    '        da.Fill(ds)
    '        ComboBox.DataSource = ds.Tables(0)
    '        ComboBox.DataValueField = ds.Tables(0).Columns(0).Caption.ToString
    '        'ComboBox.DisplayMember = ds.Tables(0).Columns(1).Caption.ToString
    '        ComboBox.DataTextField = ds.Tables(0).Columns(1).Caption.ToString
    '        'ComboBox.ValueMember = ds.Tables(0).Columns(0).Caption.ToString
    '        ComboBox.DataBind()
    '        ComboBox.SelectedIndex = -1
    '        'Catch ex As OdbcException
    '    Catch ex As Exception
    '        'MsgBox(ex.Message)
    '        'MessageBox.Show(ex.Message.ToString, _
    '        '              "error", MessageBoxButtons.OK, _
    '        '             MessageBoxIcon.Error)
    '    Finally
    '        Me.fin()
    '    End Try
    'End Sub


    'Public Function Datos(ByVal Cons As String)
    '    cn.Open()
    '    Try
    '        Dim cmd As New SqlCommand(Cons, cn)
    '        Dim da As New SqlDataAdapter(cmd)
    '        Dim dt As New DataTable
    '        da.Fill(dt)
    '        Return dt
    '    Catch ex As Exception
    '        'MsgBox(ex.Message)
    '    Finally
    '        Me.fin()
    '    End Try
    '    Return 0
    'End Function

    'Public Function Datos2(ByVal Cons2 As String)
    '    cn.Open()
    '    Try
    '        Dim insertStr As String = Cons2 & ";SELECT  Scope_Identity();"
    '        Dim cmd As New SqlCommand(insertStr, cn)
    '        Dim ret As String = cmd.ExecuteScalar().ToString()
    '        Return ret
    '    Catch ex As Exception
    '        'MsgBox(ex.Message)
    '    Finally
    '        Me.fin()
    '    End Try
    '    Return 0
    'End Function

    'Public Function Bitacora(ByVal user As Integer, ByVal cac_pdv As Integer, ByVal epsa As Integer, ByVal modulo As String, ByVal opera As String)
    '    Me.Query("INSERT INTO TJR_Bitacora(id_user, id_cac_pdv, id_empsa, modulo,operacion,fecha_hora) values(" & user & "," & cac_pdv & "," & epsa & ",'" & modulo & "','" & opera & "',sysdatetime())")
    '    Me.Ejecutar()
    '    Return 0
    'End Function

    'Public Sub Cargar_CheckBox(ByVal CheckBox As CheckBoxList, ByVal Texto As String, ByVal Valor As String, ByVal Sentencia As String, ByVal idacceso As Integer, ByVal idperf As Integer)
    '    cn.Open()
    '    Try
    '        Dim comd As New SqlCommand(Sentencia, cn)
    '        CheckBox.DataSource = comd.ExecuteReader
    '        CheckBox.DataTextField = Texto
    '        CheckBox.DataValueField = Valor
    '        CheckBox.DataBind()
    '        Me.fin()
    '        cn.Open()
    '        Dim cmd As New SqlCommand("SELECT c.Id_Control, c.Nom_control, a.Nom_acceso FROM TJR_Accesos as a INNER JOIN TJR_Asigna_Permisos  as ap ON a.Id_acceso = ap.Id_acceso INNER JOIN TJR_Controles as c ON a.Id_acceso = c.Id_acceso AND ap.Id_Control = c.Id_Control AND  ap.Id_acceso = c.Id_acceso INNER JOIN TJR_Perfiles as p ON ap.id_perfil = p.id_perfil WHERE(ap.Permiso = 'S') AND (a.Id_acceso = " & idacceso & ") AND (p.id_perfil = " & idperf & ") group by a.Nom_acceso,c.Nom_control,c.id_control order by(c.Nom_control) asc ", cn)
    '        Dim da As New SqlDataAdapter(cmd)
    '        Dim dt As New DataTable
    '        da.Fill(dt)
    '        For i = 0 To dt.Rows.Count - 1
    '            For j = 0 To CheckBox.Items.Count - 1
    '                If CheckBox.Items(j).Text = dt.Rows(i).Item("nom_control") Then
    '                    CheckBox.Items(j).Selected = True
    '                End If
    '            Next
    '        Next
    '    Catch ex As Exception
    '        'MsgBox(ex.Message)
    '    Finally
    '        Me.fin()
    '    End Try
    'End Sub
    'Public Sub Cargar_Radio(ByVal CheckBox As RadioButtonList, ByVal Valor As String, ByVal Texto As String, ByVal Sentencia As String)
    '    cn.Open()
    '    Try
    '        Dim comd As New SqlCommand(Sentencia, cn)
    '        CheckBox.DataSource = comd.ExecuteReader
    '        CheckBox.DataValueField = Valor
    '        CheckBox.DataTextField = Texto
    '        CheckBox.DataBind()
    '        Me.fin()
    '        cn.Open()
    '        Dim cmd As New SqlCommand("Select Id_Hor_Acc, Nom_Hor_acc from tjr_horario_accion where Nom_Hor_Acc ='" & Texto & "'", cn)
    '        Dim da As New SqlDataAdapter(cmd)
    '        Dim dt As New DataTable
    '        da.Fill(dt)
    '        For i = 0 To dt.Rows.Count - 1
    '            For j = 0 To CheckBox.Items.Count - 1
    '                If CheckBox.Items(j).Text = dt.Rows(i).Item("Id_Hor_Acc") Then
    '                    CheckBox.Items(j).Selected = True
    '                End If
    '            Next
    '        Next
    '    Catch ex As Exception
    '        'MsgBox(ex.Message)
    '    Finally
    '        Me.fin()
    '    End Try
    'End Sub

    'Public Function Factura_Electronica(ByVal id_enc As Integer, ByVal Caja As Integer) As String
    '    'Dim firma As String
    '    'Dim cae As String
    '    '' crea el objeto para la cabecera de la factura
    '    'Dim dte As New infile.dte
    '    '' crea el objeto para el detalle de la factura
    '    'Dim deta As New infile.detalleDte

    '    '' se crea el objeto que se encargara de registrar todo el encabezado y el detalle
    '    'Dim registro As New infile.requestDte

    '    '' se crea un objeto que sera el encargado de obtener la respuesta o resultado del ws

    '    'Dim resultado As New infile.responseDte

    '    '' ser crea el objeto de tipo servicio proxy  WS
    '    'Dim ws As New infile.ingfaceClient

    '    'Dim localDeta As New infile.detalleDte

    '    'Dim dt As DataTable
    '    'Dim dt2 As DataTable
    '    'Try
    '    '    dt = Me.Datos("select * from dbo.V_Enc_Factura where [ID] = " & id_enc & " and caja = " & Caja & " and doc = 'Factura'")

    '    '    If dt.Rows.Count > 0 Then

    '    '        'Como ejemplo ilustrativo tanto el encabezado como el detalle de la factura se les asignara valores
    '    '        ' constantes 
    '    '        ' cuando se setea del dte y el detalle existen unos campos booleans que siempre deberan ir a true
    '    '        ' ya que de no ser asi el servicio no sabra si se envio o no el dato

    '    '        dte.idDispositivo = Trim(dt.Rows(0).Item("EST").ToString)
    '    '        dte.estadoDocumento = "Activo".ToString
    '    '        dte.codigoMoneda = "GTQ".ToString
    '    '        dte.tipoDocumento = "FACE".ToString
    '    '        dte.nitComprador = Trim(dt.Rows(0).Item("nit_clt").ToString().Replace("-", "")) 'Colocar aquí un nit valido o C/F
    '    '        dte.regimen2989 = False
    '    '        dte.nitVendedor = Trim(dt.Rows(0).Item("NitEmp").ToString().Replace("-", "")) 'Colocar aquí el nit del vendedor

    '    '        dte.serieAutorizada = Trim(dt.Rows(0).Item("serie").ToString()) 'cuando se tengan las autorizaciones

    '    '        dte.montoTotalOperacion = Trim(dt.Rows(0).Item("Total").ToString)

    '    '        dte.fechaDocumento = Trim(dt.Rows(0).Item("Fecha_Fact").ToString)
    '    '        dte.fechaAnulacionSpecified = True
    '    '        dte.fechaDocumentoSpecified = True
    '    '        dte.fechaResolucionSpecified = True
    '    '        dte.tipoCambioSpecified = True
    '    '        dte.detalleImpuestosIvaSpecified = True
    '    '        dte.importeOtrosImpuestosSpecified = True
    '    '        dte.fechaAnulacion = Now.Date
    '    '        dte.numeroDocumento = id_enc.ToString 'ID para la factura
    '    '        dte.numeroDocumento = Trim("".ToString)
    '    '        dte.telefonoComprador = "n/a"
    '    '        dte.importeDescuento = Convert.ToDouble(dt.Rows(0).Item("Descuento"))
    '    '        dte.importeDescuentoSpecified = True
    '    '        dte.importeTotalExento = Convert.ToDouble(0.0)
    '    '        dte.importeTotalExentoSpecified = True
    '    '        dte.importeNetoGravado = Convert.ToDouble(dt.Rows(0).Item("Total").ToString)
    '    '        dte.importeNetoGravadoSpecified = True
    '    '        dte.detalleImpuestosIva = Convert.ToDouble(dt.Rows(0).Item("Iva").ToString)
    '    '        dte.tipoCambio = Convert.ToDouble(1.0)
    '    '        dte.direccionComercialComprador = dt.Rows(0).Item("dir").ToString
    '    '        dte.serieDocumento = "63"
    '    '        dte.importeOtrosImpuestos = Convert.ToDouble(0.0)
    '    '        dte.numeroResolucion = Trim(dt.Rows(0).Item("Autorizacion").ToString) 'Colocar aquí el numero de la resolucion
    '    '        dte.municipioComprador = Trim(dt.Rows(0).Item("Nom_Muni").ToString)
    '    '        dte.nombreComercialComprador = Trim(dt.Rows(0).Item("Cliente").ToString)
    '    '        dte.departamentoComprador = Trim(dt.Rows(0).Item("Nom_Dep").ToString)
    '    '        dte.nombreComercialRazonSocialVendedor = Trim(dt.Rows(0).Item("razon").ToString)
    '    '        dte.nombreCompletoVendedor = Trim(dt.Rows(0).Item("Empresa").ToString)
    '    '        dte.municipioVendedor = "Guatemala"
    '    '        dte.departamentoVendedor = "Guatemala"
    '    '        dte.direccionComercialVendedor = Trim(dt.Rows(0).Item("Dire_Cac_Pdv").ToString)
    '    '        dte.fechaResolucion = Convert.ToDateTime(dt.Rows(0).Item("FAutoriza").ToString) 'Colocar aquí fecha resolucion
    '    '        dte.regimenISR = "ret definitiva"
    '    '        dte.importeBruto = Trim(dt.Rows(0).Item("sinIva").ToString)
    '    '        dte.importeBrutoSpecified = True
    '    '        dte.nitGFACE = "12521337"
    '    '        dte.codigoEstablecimiento = Trim(dt.Rows(0).Item("EST").ToString) 'Colocar aui el codigo del establecimiento 
    '    '        dte.correoComprador = "n/a"
    '    '        dte.descripcionOtroImpuesto = "n/a"
    '    '        dte.observaciones = ""

    '    '    End If


    '    '    dt2 = Me.Datos("select * from v_det_factura where ID_E = " & id_enc)
    '    '    If dt2.Rows.Count > 0 Then
    '    '        For i As Integer = 0 To dt2.Rows.Count - 1
    '    '            localDeta.cantidadSpecified = True
    '    '            localDeta.cantidad = dt2.Rows(i).Item("Cantidad")
    '    '            localDeta.codigoProducto = dt2.Rows(i).Item("ID_A")
    '    '            localDeta.detalleImpuestosIvaSpecified = True
    '    '            localDeta.descripcionProducto = dt2.Rows(i).Item("Artic")
    '    '            localDeta.montoBruto = dt2.Rows(i).Item("sinIva")
    '    '            localDeta.precioUnitario = Convert.ToDouble(dt2.Rows(i).Item("Precio"))
    '    '            localDeta.precioUnitarioSpecified = True
    '    '            localDeta.importeExento = Convert.ToDouble(0.0)
    '    '            localDeta.importeExentoSpecified = True
    '    '            localDeta.importeNetoGravado = Convert.ToDouble(dt2.Rows(i).Item("subTotal"))
    '    '            localDeta.importeNetoGravadoSpecified = True
    '    '            localDeta.importeTotalOperacion = Convert.ToDouble(dt2.Rows(i).Item("subTotal"))
    '    '            localDeta.montoDescuento = Convert.ToDouble(dt2.Rows(i).Item("Descuento"))
    '    '            localDeta.montoBrutoSpecified = True
    '    '            localDeta.montoDescuentoSpecified = True
    '    '            localDeta.importeTotalOperacionSpecified = True
    '    '            localDeta.importeOtrosImpuestosSpecified = True
    '    '            localDeta.unidadMedida = "UND"
    '    '            localDeta.detalleImpuestosIva = Convert.ToDouble(dt2.Rows(i).Item("Iva"))
    '    '            If dt2.Rows(i).Item("ProServ").ToString = "Producto" Then
    '    '                localDeta.tipoProducto = "B"

    '    '            End If
    '    '            If dt2.Rows(i).Item("ProServ").ToString = "Servicio" Then
    '    '                localDeta.tipoProducto = "S"

    '    '            End If
    '    '            localDeta.importeOtrosImpuestos = Convert.ToDouble(0.0)

    '    '            ReDim Preserve dte.detalleDte(i)

    '    '            dte.detalleDte(i) = localDeta

    '    '            localDeta = New infile.detalleDte
    '    '        Next
    '    '    End If


    '    '    ' registro es un objeto que cuenta con una propiedad dte por lo que en este punto se carga todo la factura
    '    '    ' con su encabezado y todo su detalle
    '    '    ' ademas cuenta con 2 propiedades usuario y clave, que le sirven al servicio para identificar quien esta  mandado
    '    '    ' el documento electronico
    '    '    registro.dte = dte


    '    '    registro.usuario = "DEMO" 'Colocar aquí el usuario
    '    '    registro.clave = "B2FDC80789AFAF22C372965901B16DF533A4FCB19FD9F2FD5CBDA554032983B0" 'Colocar aquí la clave 
    '    '    System.Net.ServicePointManager.Expect100Continue = False


    '    '    ' en este punto se manda todo el dte con su usario y clave al web serice,  este a su vez le pasa el resultado
    '    '    'que puede ser exitoso o no

    '    '    resultado = ws.registrarDte(registro)

    '    '    ' el objeto resultado cuenta con una propiedad llamado valido la cual es booleana  retorna true en caso
    '    '    ' de ser generado el documento elecctronico y false en caso no se pudo generar
    '    '    ' tambien cuenta con metodo descripcion que indica el motivo por el cual no se pudo generar el documento
    '    '    'electronico
    '    '    If (resultado.valido) Then

    '    '        firma = resultado.numeroDte ' le envia el numero de documento electronico al textbox1
    '    '        cae = resultado.cae       ' le envia el CAE al textbox2
    '    '        If firma = "" Or cae = "" Then
    '    '            Return "0"
    '    '        Else
    '    '            Me.Query("update tjr_enc_Factura set firma = '" & Trim(firma) & "', cae = '" & Trim(cae) & "' where id_enc = " & id_enc)
    '    '            Me.Ejecutar()
    '    '            Return "1"
    '    '        End If
    '    '    Else
    '    '        'MsgBox("RETURN")
    '    '        'MsgBox(resultado.descripcion)   ' saca una ventana indicando del por que no se pudo generar el documento
    '    '        Return resultado.descripcion
    '    '    End If

    '    'Catch ex As Exception
    '    '    'MsgBox("CATCH")
    '    '    Return 0
    '    'End Try

    'End Function

    'Public Function Factura_Electronica_Copia(ByVal id_enc As Integer, ByVal Caja As Integer) As String
    '    'Dim firma As String
    '    'Dim cae As String
    '    '' crea el objeto para la cabecera de la factura
    '    'Dim dte As New infile.dte
    '    '' crea el objeto para el detalle de la factura
    '    'Dim deta As New infile.detalleDte

    '    '' se crea el objeto que se encargara de registrar todo el encabezado y el detalle
    '    'Dim registro As New infile.requestDte

    '    '' se crea un objeto que sera el encargado de obtener la respuesta o resultado del ws

    '    'Dim resultado As New infile.responseDte

    '    '' ser crea el objeto de tipo servicio proxy  WS
    '    'Dim ws As New infile.ingfaceClient

    '    'Dim localDeta As New infile.detalleDte

    '    'Dim dt As DataTable
    '    'Dim dt2 As DataTable
    '    'Try
    '    '    dt = Me.Datos("select * from dbo.V_Enc_Factura where [ID] = " & id_enc & " and caja = " & Caja & " and doc = 'Factura Copia'")

    '    '    If dt.Rows.Count > 0 Then

    '    '        'Como ejemplo ilustrativo tanto el encabezado como el detalle de la factura se les asignara valores
    '    '        ' constantes 
    '    '        ' cuando se setea del dte y el detalle existen unos campos booleans que siempre deberan ir a true
    '    '        ' ya que de no ser asi el servicio no sabra si se envio o no el dato

    '    '        dte.idDispositivo = Trim(dt.Rows(0).Item("EST").ToString)
    '    '        dte.estadoDocumento = "Activo".ToString
    '    '        dte.codigoMoneda = "GTQ".ToString
    '    '        dte.tipoDocumento = "CFACE".ToString
    '    '        dte.nitComprador = dt.Rows(0).Item("nit_clt").ToString().Replace("-", "") 'Colocar aquí un nit valido o C/F
    '    '        dte.regimen2989 = False
    '    '        dte.nitVendedor = dt.Rows(0).Item("NitEmp").ToString().Replace("-", "") 'Colocar aquí el nit del vendedor

    '    '        dte.serieAutorizada = Trim(dt.Rows(0).Item("serie").ToString()) 'cuando se tengan las autorizaciones

    '    '        dte.montoTotalOperacion = dt.Rows(0).Item("Total").ToString

    '    '        dte.fechaDocumento = dt.Rows(0).Item("Fecha_Fact").ToString
    '    '        dte.fechaAnulacionSpecified = True
    '    '        dte.fechaDocumentoSpecified = True
    '    '        dte.fechaResolucionSpecified = True
    '    '        dte.tipoCambioSpecified = True
    '    '        dte.detalleImpuestosIvaSpecified = True
    '    '        dte.importeOtrosImpuestosSpecified = True
    '    '        dte.fechaAnulacion = Now.Date
    '    '        dte.numeroDocumento = id_enc.ToString 'ID para la factura
    '    '        dte.numeroDocumento = Trim(dt.Rows(0).Item("Numero").ToString)
    '    '        dte.telefonoComprador = "n/a"
    '    '        dte.importeDescuento = Convert.ToDouble(dt.Rows(0).Item("Descuento"))
    '    '        dte.importeDescuentoSpecified = True
    '    '        dte.importeTotalExento = Convert.ToDouble(0.0)
    '    '        dte.importeTotalExentoSpecified = True
    '    '        dte.importeNetoGravado = Convert.ToDouble(dt.Rows(0).Item("Total").ToString)
    '    '        dte.importeNetoGravadoSpecified = True
    '    '        dte.detalleImpuestosIva = Convert.ToDouble(dt.Rows(0).Item("Iva").ToString)
    '    '        dte.tipoCambio = Convert.ToDouble(1.0)
    '    '        dte.direccionComercialComprador = dt.Rows(0).Item("dir").ToString
    '    '        dte.serieDocumento = "53"
    '    '        dte.importeOtrosImpuestos = Convert.ToDouble(0.0)
    '    '        dte.numeroResolucion = dt.Rows(0).Item("Autorizacion").ToString 'Colocar aquí el numero de la resolucion
    '    '        dte.municipioComprador = dt.Rows(0).Item("Nom_Muni").ToString
    '    '        dte.nombreComercialComprador = dt.Rows(0).Item("Cliente").ToString
    '    '        dte.departamentoComprador = dt.Rows(0).Item("Nom_Dep").ToString
    '    '        dte.nombreComercialRazonSocialVendedor = dt.Rows(0).Item("razon").ToString
    '    '        dte.nombreCompletoVendedor = dt.Rows(0).Item("Empresa").ToString
    '    '        dte.municipioVendedor = "Guatemala"
    '    '        dte.departamentoVendedor = "Guatemala"
    '    '        dte.direccionComercialVendedor = dt.Rows(0).Item("Dire_Cac_Pdv").ToString
    '    '        dte.fechaResolucion = Convert.ToDateTime(dt.Rows(0).Item("FAutoriza").ToString) 'Colocar aquí fecha resolucion
    '    '        dte.regimenISR = "ret definitiva"
    '    '        dte.importeBruto = dt.Rows(0).Item("sinIva").ToString
    '    '        dte.importeBrutoSpecified = True
    '    '        dte.nitGFACE = "12521337"
    '    '        dte.codigoEstablecimiento = Trim(dt.Rows(0).Item("EST").ToString) 'Colocar aui el codigo del establecimiento 
    '    '        dte.correoComprador = "n/a"
    '    '        dte.descripcionOtroImpuesto = "n/a"
    '    '        dte.observaciones = ""
    '    '        dte.estadoDocumento = "ACTIVO"

    '    '    End If


    '    '    dt2 = Me.Datos("select * from v_det_factura where ID_E = " & id_enc)
    '    '    If dt2.Rows.Count > 0 Then
    '    '        For i As Integer = 0 To dt2.Rows.Count - 1
    '    '            localDeta.cantidadSpecified = True
    '    '            localDeta.cantidad = dt2.Rows(0).Item("Cantidad")
    '    '            localDeta.codigoProducto = dt2.Rows(0).Item("ID_A")
    '    '            localDeta.detalleImpuestosIvaSpecified = True
    '    '            localDeta.descripcionProducto = dt2.Rows(0).Item("Artic")
    '    '            localDeta.montoBruto = dt2.Rows(0).Item("sinIva")
    '    '            localDeta.precioUnitario = Convert.ToDouble(dt2.Rows(0).Item("Precio"))
    '    '            localDeta.precioUnitarioSpecified = True
    '    '            localDeta.importeExento = Convert.ToDouble(0.0)
    '    '            localDeta.importeExentoSpecified = True
    '    '            localDeta.importeNetoGravado = Convert.ToDouble(dt2.Rows(0).Item("subTotal"))
    '    '            localDeta.importeNetoGravadoSpecified = True
    '    '            localDeta.importeTotalOperacion = Convert.ToDouble(dt2.Rows(0).Item("subTotal"))
    '    '            localDeta.montoDescuento = Convert.ToDouble(dt2.Rows(0).Item("Descuento"))
    '    '            localDeta.montoBrutoSpecified = True
    '    '            localDeta.montoDescuentoSpecified = True
    '    '            localDeta.importeTotalOperacionSpecified = True
    '    '            localDeta.importeOtrosImpuestosSpecified = True
    '    '            localDeta.unidadMedida = "UND"
    '    '            localDeta.detalleImpuestosIva = Convert.ToDouble(dt2.Rows(0).Item("Iva"))
    '    '            If dt2.Rows(0).Item("ProServ").ToString = "Producto" Then
    '    '                localDeta.tipoProducto = "B"

    '    '            End If
    '    '            If dt2.Rows(0).Item("ProServ").ToString = "Servicio" Then
    '    '                localDeta.tipoProducto = "S"

    '    '            End If
    '    '            localDeta.importeOtrosImpuestos = Convert.ToDouble(0.0)

    '    '            ReDim Preserve dte.detalleDte(i)

    '    '            dte.detalleDte(i) = localDeta

    '    '            'localDeta = New infile.detalleDte
    '    '        Next
    '    '    End If


    '    '    ' registro es un objeto que cuenta con una propiedad dte por lo que en este punto se carga todo la factura
    '    '    ' con su encabezado y todo su detalle
    '    '    ' ademas cuenta con 2 propiedades usuario y clave, que le sirven al servicio para identificar quien esta  mandado
    '    '    ' el documento electronico
    '    '    registro.dte = dte


    '    '    registro.usuario = "DEMO" 'Colocar aquí el usuario
    '    '    registro.clave = "B2FDC80789AFAF22C372965901B16DF533A4FCB19FD9F2FD5CBDA554032983B0" 'Colocar aquí la clave 
    '    '    System.Net.ServicePointManager.Expect100Continue = False


    '    '    ' en este punto se manda todo el dte con su usario y clave al web serice,  este a su vez le pasa el resultado
    '    '    'que puede ser exitoso o no

    '    '    resultado = ws.registrarDte(registro)

    '    '    ' el objeto resultado cuenta con una propiedad llamado valido la cual es booleana  retorna true en caso
    '    '    ' de ser generado el documento elecctronico y false en caso no se pudo generar
    '    '    ' tambien cuenta con metodo descripcion que indica el motivo por el cual no se pudo generar el documento
    '    '    'electronico


    '    '    If (resultado.valido) Then
    '    '        firma = resultado.numeroDte ' le envia el numero de documento electronico al textbox1
    '    '        cae = resultado.cae       ' le envia el CAE al textbox2
    '    '        If firma = "" Or cae = "" Then
    '    '            Return "0"
    '    '        Else
    '    '            Me.Query("update tjr_enc_Factura set firma = '" & Trim(firma) & "', cae = '" & Trim(cae) & "' where id_enc = " & id_enc)
    '    '            Me.Ejecutar()
    '    '            Return "1"
    '    '        End If
    '    '    Else
    '    '        'MsgBox("PROBANDO")
    '    '        'MsgBox(resultado.descripcion)   'saca una ventana indicando del por que no se pudo generar el documento
    '    '        Return resultado.descripcion
    '    '    End If
    '    '    ' fin del tutoria
    '    '    ' para mas informacion comuniquese con Infile, S.A.
    '    '    ' al departamento de IT.
    '    '    ' o al mail ,   crsandoval@infile.com.gt
    '    'Catch ex As Exception
    '    '    Return "0"
    '    'End Try

    'End Function

    'Public Function Factura_Electronica_Copia_Anulada(ByVal id_enc As Integer, ByVal Caja As Integer) As String
    '    'Dim firma As String
    '    'Dim cae As String
    '    '' crea el objeto para la cabecera de la factura
    '    'Dim dte As New infile.dte
    '    '' crea el objeto para el detalle de la factura
    '    'Dim deta As New infile.detalleDte

    '    '' se crea el objeto que se encargara de registrar todo el encabezado y el detalle
    '    'Dim registro As New infile.requestDte

    '    '' se crea un objeto que sera el encargado de obtener la respuesta o resultado del ws

    '    'Dim resultado As New infile.responseDte

    '    '' ser crea el objeto de tipo servicio proxy  WS
    '    'Dim ws As New infile.ingfaceClient

    '    'Dim localDeta As New infile.detalleDte

    '    'Dim dt As DataTable
    '    'Dim dt2 As DataTable
    '    'Try
    '    '    dt = Me.Datos("select * from dbo.V_Enc_Factura where [ID] = " & id_enc & " and caja = " & Caja & " and doc = 'Factura Copia'")

    '    '    If dt.Rows.Count > 0 Then

    '    '        'Como ejemplo ilustrativo tanto el encabezado como el detalle de la factura se les asignara valores
    '    '        ' constantes 
    '    '        ' cuando se setea del dte y el detalle existen unos campos booleans que siempre deberan ir a true
    '    '        ' ya que de no ser asi el servicio no sabra si se envio o no el dato

    '    '        dte.idDispositivo = Trim(dt.Rows(0).Item("EST").ToString)
    '    '        dte.estadoDocumento = "Activo".ToString
    '    '        dte.codigoMoneda = "GTQ".ToString
    '    '        dte.tipoDocumento = "CFACE".ToString
    '    '        dte.nitComprador = dt.Rows(0).Item("nit_clt").ToString().Replace("-", "") 'Colocar aquí un nit valido o C/F
    '    '        dte.regimen2989 = False
    '    '        dte.nitVendedor = dt.Rows(0).Item("NitEmp").ToString().Replace("-", "") 'Colocar aquí el nit del vendedor

    '    '        dte.serieAutorizada = Trim(dt.Rows(0).Item("serie").ToString()) 'cuando se tengan las autorizaciones

    '    '        dte.montoTotalOperacion = dt.Rows(0).Item("Total").ToString

    '    '        dte.fechaDocumento = dt.Rows(0).Item("Fecha_fact").ToString
    '    '        dte.fechaAnulacionSpecified = True
    '    '        dte.fechaDocumentoSpecified = True
    '    '        dte.fechaResolucionSpecified = True
    '    '        dte.tipoCambioSpecified = True
    '    '        dte.detalleImpuestosIvaSpecified = True
    '    '        dte.importeOtrosImpuestosSpecified = True
    '    '        dte.fechaAnulacion = Now.Date
    '    '        dte.numeroDocumento = id_enc.ToString 'ID para la factura
    '    '        dte.numeroDocumento = Trim(dt.Rows(0).Item("Numero").ToString)
    '    '        dte.telefonoComprador = "n/a"
    '    '        dte.importeDescuento = Convert.ToDouble(dt.Rows(0).Item("Descuento"))
    '    '        dte.importeDescuentoSpecified = True
    '    '        dte.importeTotalExento = Convert.ToDouble(0.0)
    '    '        dte.importeTotalExentoSpecified = True
    '    '        dte.importeNetoGravado = Convert.ToDouble(dt.Rows(0).Item("Total").ToString)
    '    '        dte.importeNetoGravadoSpecified = True
    '    '        dte.detalleImpuestosIva = Convert.ToDouble(dt.Rows(0).Item("Iva").ToString)
    '    '        dte.tipoCambio = Convert.ToDouble(1.0)
    '    '        dte.direccionComercialComprador = dt.Rows(0).Item("dir").ToString
    '    '        dte.serieDocumento = "53"
    '    '        dte.importeOtrosImpuestos = Convert.ToDouble(0.0)
    '    '        dte.numeroResolucion = dt.Rows(0).Item("Autorizacion").ToString 'Colocar aquí el numero de la resolucion
    '    '        dte.municipioComprador = dt.Rows(0).Item("Nom_Muni").ToString
    '    '        dte.nombreComercialComprador = dt.Rows(0).Item("Cliente").ToString
    '    '        dte.departamentoComprador = dt.Rows(0).Item("Nom_Dep").ToString
    '    '        dte.nombreComercialRazonSocialVendedor = dt.Rows(0).Item("razon").ToString
    '    '        dte.nombreCompletoVendedor = dt.Rows(0).Item("Empresa").ToString
    '    '        dte.municipioVendedor = "Guatemala"
    '    '        dte.departamentoVendedor = "Guatemala"
    '    '        dte.direccionComercialVendedor = dt.Rows(0).Item("Dire_Cac_Pdv").ToString
    '    '        dte.fechaResolucion = Convert.ToDateTime(dt.Rows(0).Item("FAutoriza").ToString) 'Colocar aquí fecha resolucion
    '    '        dte.regimenISR = "ret definitiva"
    '    '        dte.importeBruto = dt.Rows(0).Item("sinIva").ToString
    '    '        dte.importeBrutoSpecified = True
    '    '        dte.nitGFACE = "12521337"
    '    '        dte.codigoEstablecimiento = Trim(dt.Rows(0).Item("EST").ToString) 'Colocar aui el codigo del establecimiento 
    '    '        dte.correoComprador = "n/a"
    '    '        dte.descripcionOtroImpuesto = "n/a"
    '    '        dte.observaciones = ""
    '    '        dte.estadoDocumento = "ANULADO"
    '    '        dte.fechaAnulacion = dt.Rows(0).Item("Fecha_fact").ToString
    '    '    End If


    '    '    dt2 = Me.Datos("select * from v_det_factura where ID_E = " & id_enc)
    '    '    If dt2.Rows.Count > 0 Then
    '    '        For i As Integer = 0 To dt2.Rows.Count - 1
    '    '            localDeta.cantidadSpecified = True
    '    '            localDeta.cantidad = dt2.Rows(i).Item("Cantidad")
    '    '            localDeta.codigoProducto = dt2.Rows(i).Item("ID_A")
    '    '            localDeta.detalleImpuestosIvaSpecified = True
    '    '            localDeta.descripcionProducto = dt2.Rows(i).Item("Artic")
    '    '            localDeta.montoBruto = dt2.Rows(i).Item("sinIva")
    '    '            localDeta.precioUnitario = Convert.ToDouble(dt2.Rows(i).Item("Precio"))
    '    '            localDeta.precioUnitarioSpecified = True
    '    '            localDeta.importeExento = Convert.ToDouble(0.0)
    '    '            localDeta.importeExentoSpecified = True
    '    '            localDeta.importeNetoGravado = Convert.ToDouble(dt2.Rows(i).Item("subTotal"))
    '    '            localDeta.importeNetoGravadoSpecified = True
    '    '            localDeta.importeTotalOperacion = Convert.ToDouble(dt2.Rows(i).Item("subTotal"))
    '    '            localDeta.montoDescuento = Convert.ToDouble(dt2.Rows(i).Item("Descuento"))
    '    '            localDeta.montoBrutoSpecified = True
    '    '            localDeta.montoDescuentoSpecified = True
    '    '            localDeta.importeTotalOperacionSpecified = True
    '    '            localDeta.importeOtrosImpuestosSpecified = True
    '    '            localDeta.unidadMedida = "UND"
    '    '            localDeta.detalleImpuestosIva = Convert.ToDouble(dt2.Rows(i).Item("Iva"))
    '    '            If dt2.Rows(i).Item("ProServ").ToString = "Producto" Then
    '    '                localDeta.tipoProducto = "B"

    '    '            End If
    '    '            If dt2.Rows(i).Item("ProServ").ToString = "Servicio" Then
    '    '                localDeta.tipoProducto = "S"

    '    '            End If
    '    '            localDeta.importeOtrosImpuestos = Convert.ToDouble(0.0)

    '    '            ReDim Preserve dte.detalleDte(i)

    '    '            dte.detalleDte(i) = localDeta

    '    '            localDeta = New infile.detalleDte
    '    '        Next
    '    '    End If


    '    '    ' registro es un objeto que cuenta con una propiedad dte por lo que en este punto se carga todo la factura
    '    '    ' con su encabezado y todo su detalle
    '    '    ' ademas cuenta con 2 propiedades usuario y clave, que le sirven al servicio para identificar quien esta  mandado
    '    '    ' el documento electronico
    '    '    registro.dte = dte


    '    '    registro.usuario = "DEMO" 'Colocar aquí el usuario
    '    '    registro.clave = "B2FDC80789AFAF22C372965901B16DF533A4FCB19FD9F2FD5CBDA554032983B0" 'Colocar aquí la clave 
    '    '    System.Net.ServicePointManager.Expect100Continue = False


    '    '    ' en este punto se manda todo el dte con su usario y clave al web serice,  este a su vez le pasa el resultado
    '    '    'que puede ser exitoso o no

    '    '    resultado = ws.registrarDte(registro)

    '    '    ' el objeto resultado cuenta con una propiedad llamado valido la cual es booleana  retorna true en caso
    '    '    ' de ser generado el documento elecctronico y false en caso no se pudo generar
    '    '    ' tambien cuenta con metodo descripcion que indica el motivo por el cual no se pudo generar el documento
    '    '    'electronico


    '    '    If (resultado.valido) Then
    '    '        firma = resultado.numeroDte ' le envia el numero de documento electronico al textbox1
    '    '        cae = resultado.cae       ' le envia el CAE al textbox2
    '    '        If firma = "" Or cae = "" Then
    '    '            Return "0"
    '    '        Else
    '    '            Me.Query("update tjr_enc_Factura set firma = '" & Trim(firma) & "', cae = '" & Trim(cae) & "' where id_enc = " & id_enc)
    '    '            Me.Ejecutar()
    '    '            Return "1"
    '    '        End If
    '    '    Else
    '    '        'MsgBox("PROBANDO")
    '    '        'MsgBox(resultado.descripcion)   'saca una ventana indicando del por que no se pudo generar el documento
    '    '        Return resultado.descripcion
    '    '    End If
    '    '    ' fin del tutoria
    '    '    ' para mas informacion comuniquese con Infile, S.A.
    '    '    ' al departamento de IT.
    '    '    ' o al mail ,   crsandoval@infile.com.gt
    '    'Catch ex As Exception
    '    '    Return "0"
    '    'End Try

    'End Function

    'Private Sub fin()
    '    Try
    '        cn.Close()
    '    Catch ex As Exception
    '        'MsgBox(ex.Message)
    '    End Try
    'End Sub

    Public Class encabezadoNotD
        Public EST As Integer
        Public nit_clt As String
        Public Nit_Emp As String
        Public serie As String
        Public Total As Double
        Public id_enc As Integer
        Public Descuento As Double
        Public Iva As Double
        Public dir As String
        Public Autorizacion As String
        Public Nom_Muni As String
        Public Cliente As String
        Public Nom_Dep As String
        Public razon As String
        Public Empresa As String
        Public Dir_CAC As String
        Public FAutoriza As String
        Public sin_Iva As String
        Public obs As String
    End Class



    'Public Function NotaCredito(ByVal id_enc As Integer, ByVal caja As Integer) As String
    '    Dim firma As String
    '    Dim cae As String
    '    ' crea el objeto para la cabecera de la factura
    '    'Dim dte As New infile.dte
    '    ' crea el objeto para el detalle de la factura
    '    'Dim deta As New infile.detalleDte

    '    ' se crea el objeto que se encargara de registrar todo el encabezado y el detalle
    '    'Dim registro As New infile.requestDte

    '    ' se crea un objeto que sera el encargado de obtener la respuesta o resultado del ws

    '    'Dim resultado As New infile.responseDte

    '    ' ser crea el objeto de tipo servicio proxy  WS
    '    'Dim ws As New infile.ingfaceClient

    '    'Dim localDeta As New infile.detalleDte

    '    Dim dt As DataTable
    '    Try

    '        dt = Me.Datos("select * from dbo.V_Notas_Credito where [ID_enc] = " & id_enc & " and caja = " & caja & " and doc = 'Nota Credito'")

    '        If dt.Rows.Count > 0 Then

    '            'Como ejemplo ilustrativo tanto el encabezado como el detalle de la factura se les asignara valores
    '            ' constantes 
    '            ' cuando se setea del dte y el detalle existen unos campos booleans que siempre deberan ir a true
    '            ' ya que de no ser asi el servicio no sabra si se envio o no el dato

    '            'dte.idDispositivo = Trim(dt.Rows(0).Item("EST").ToString)
    '            'dte.estadoDocumento = "Activo".ToString
    '            'dte.codigoMoneda = "GTQ".ToString
    '            'dte.tipoDocumento = "NCE".ToString
    '            'dte.nitComprador = dt.Rows(0).Item("nit_clt").ToString().Replace("-", "") 'Colocar aquí un nit valido o C/F
    '            'dte.regimen2989 = False
    '            'dte.nitVendedor = dt.Rows(0).Item("Nit_Emp").ToString().Replace("-", "") 'Colocar aquí el nit del vendedor

    '            'dte.serieAutorizada = Trim(dt.Rows(0).Item("serie").ToString()) 'cuando se tengan las autorizaciones

    '            'dte.montoTotalOperacion = dt.Rows(0).Item("Total").ToString

    '            'dte.fechaDocumento = dt.Rows(0).Item("Fecha_Fact").ToString
    '            'dte.fechaAnulacionSpecified = True
    '            'dte.fechaDocumentoSpecified = True
    '            'dte.fechaResolucionSpecified = True
    '            'dte.tipoCambioSpecified = True
    '            'dte.detalleImpuestosIvaSpecified = True
    '            'dte.importeOtrosImpuestosSpecified = True
    '            'dte.fechaAnulacion = Now.Date
    '            'dte.numeroDocumento = id_enc.ToString 'ID para la factura
    '            'dte.numeroDocumento = Trim("".ToString)
    '            'dte.telefonoComprador = "n/a"
    '            'dte.importeDescuento = Convert.ToDouble(dt.Rows(0).Item("Descuento"))
    '            'dte.importeDescuentoSpecified = True
    '            'dte.importeTotalExento = Convert.ToDouble(0.0)
    '            'dte.importeTotalExentoSpecified = True
    '            'dte.importeNetoGravado = Convert.ToDouble(dt.Rows(0).Item("Total").ToString)
    '            'dte.importeNetoGravadoSpecified = True
    '            'dte.detalleImpuestosIva = Convert.ToDouble(dt.Rows(0).Item("Iva").ToString)
    '            'dte.tipoCambio = Convert.ToDouble(1.0)
    '            'dte.direccionComercialComprador = dt.Rows(0).Item("dir").ToString
    '            'dte.serieDocumento = "64"
    '            'dte.importeOtrosImpuestos = Convert.ToDouble(0.0)
    '            'dte.numeroResolucion = dt.Rows(0).Item("Autorizacion").ToString().Replace("-", "") 'Colocar aquí el numero de la resolucion
    '            'dte.municipioComprador = dt.Rows(0).Item("Nom_Muni").ToString
    '            'dte.nombreComercialComprador = dt.Rows(0).Item("Cliente").ToString
    '            'dte.departamentoComprador = dt.Rows(0).Item("Nom_Dep").ToString
    '            'dte.nombreComercialRazonSocialVendedor = dt.Rows(0).Item("razon").ToString
    '            'dte.nombreCompletoVendedor = dt.Rows(0).Item("Empresa").ToString
    '            'dte.municipioVendedor = "Guatemala"
    '            'dte.departamentoVendedor = "Guatemala"
    '            'dte.direccionComercialVendedor = dt.Rows(0).Item("Dir_CAC").ToString
    '            'dte.fechaResolucion = Convert.ToDateTime(dt.Rows(0).Item("FAutoriza").ToString) 'Colocar aquí fecha resolucion
    '            'dte.regimenISR = "ret definitiva"
    '            'dte.importeBruto = dt.Rows(0).Item("sin_Iva").ToString
    '            'dte.importeBrutoSpecified = True
    '            'dte.nitGFACE = "12521337"
    '            'dte.codigoEstablecimiento = Trim(dt.Rows(0).Item("EST").ToString) 'Colocar aui el codigo del establecimiento 
    '            'dte.correoComprador = "n/a"
    '            'dte.descripcionOtroImpuesto = "n/a"
    '            'dte.observaciones = dt.Rows(0).Item("obs").ToString

    '        End If

    '        ' registro es un objeto que cuenta con una propiedad dte por lo que en este punto se carga todo la factura
    '        ' con su encabezado y todo su detalle
    '        ' ademas cuenta con 2 propiedades usuario y clave, que le sirven al servicio para identificar quien esta  mandado
    '        ' el documento electronico
    '        'registro.dte = dte


    '        'registro.usuario = "DEMO" 'Colocar aquí el usuario
    '        'registro.clave = "B2FDC80789AFAF22C372965901B16DF533A4FCB19FD9F2FD5CBDA554032983B0" 'Colocar aquí la clave 
    '        'System.Net.ServicePointManager.Expect100Continue = False


    '        ' en este punto se manda todo el dte con su usario y clave al web serice,  este a su vez le pasa el resultado
    '        'que puede ser exitoso o no

    '        'resultado = ws.registrarDte(registro)

    '        ' el objeto resultado cuenta con una propiedad llamado valido la cual es booleana  retorna true en caso
    '        ' de ser generado el documento elecctronico y false en caso no se pudo generar
    '        ' tambien cuenta con metodo descripcion que indica el motivo por el cual no se pudo generar el documento
    '        ''electronico
    '        'If (resultado.valido) Then

    '        '    firma = resultado.numeroDte ' le envia el numero de documento electronico al textbox1
    '        '    cae = resultado.cae       ' le envia el CAE al textbox2
    '        '    If firma = "" Or cae = "" Then
    '        '        Return "0"
    '        '    Else
    '        '        Me.Query("update tjr_enc_Nota set firma = '" & Trim(firma) & "', cae = '" & Trim(cae) & "' where id_enc = " & id_enc)
    '        '        Me.Ejecutar()
    '        '        Return "1"
    '        '    End If

    '        'Else
    '        '    'MsgBox(resultado.descripcion)   ' saca una ventana indicando del por que no se pudo generar el documento
    '        '    Return resultado.descripcion
    '        'End If
    '    Catch ex As Exception
    '        Return 0
    '    End Try
    'End Function

    'Public Function CreaPOS(ByVal id_enc As Integer, ByVal caja As Integer) As String
    '    Dim dt As DataTable = Me.Datos("select * from dbo.V_Enc_Factura where [ID] = " & id_enc & " and doc='Factura' and caja = " & caja)
    '    Dim dt2 As DataTable = Me.Datos("select * from v_det_factura where ID_E = " & id_enc)
    '    '******************************CREACION DEL ARCHIVO POS***********************************'
    '    Dim ArchivoPos As String = "factura" & dt.Rows(0).Item("Numero").ToString & ".pos"
    '    Dim ruta As String = HttpContext.Current.Server.MapPath("~/Impreso/" & ArchivoPos)
    '    Dim TextFile As New FileInfo(ruta)
    '    Dim corr, auto, nemp() As String
    '    nemp = Split(dt.Rows(0).Item("nom_emp").ToString, " ")
    '    Dim Fichero As StreamWriter = TextFile.CreateText
    '    Dim GuiaImpresion As String = dt.Rows(0).Item("numero").ToString
    '    'NUEVO ARCHIVO DE BOLETOS
    '    Fichero.WriteLine("")
    '    Fichero.WriteLine("#Guia#No")
    '    Fichero.WriteLine("#Factura#Si")
    '    Fichero.WriteLine("#Boleto#No")
    '    Fichero.WriteLine("#PaseDeAbordaje#No")
    '    Fichero.WriteLine("#BARCODE#" + GuiaImpresion.ToString)
    '    Fichero.WriteLine("#TipoPago#Contado")
    '    Fichero.WriteLine("#Paquetes#-")
    '    Fichero.WriteLine("#Sobres#-")
    '    auto = Day(dt.Rows(0).Item("FAutoriza").ToString) & "/" & Month(dt.Rows(0).Item("FAutoriza").ToString) & "/" & Year(dt.Rows(0).Item("FAutoriza").ToString)
    '    corr = "DEL " & dt.Rows(0).Item("ini").ToString & " AL " & dt.Rows(0).Item("fin").ToString
    '    Fichero.WriteLine("#Correlativos#" & corr)
    '    Fichero.WriteLine("#Serie#" + dt.Rows(0).Item("Serie").ToString)
    '    Fichero.WriteLine("#NumFactura#" + dt.Rows(0).Item("Numero").ToString)
    '    Fichero.WriteLine("#FechaAutorizacion#" + auto)
    '    Fichero.WriteLine("#NombreEmpresa#" + dt.Rows(0).Item("Empresa").ToString)
    '    Fichero.WriteLine("#NitEmpresa#" + dt.Rows(0).Item("NitEmp").ToString)
    '    Fichero.WriteLine("#NombreComercial#" + dt.Rows(0).Item("Razon").ToString)
    '    Fichero.WriteLine("#DireccionAgencia#" + dt.Rows(0).Item("Dire_Cac_Pdv").ToString)
    '    'Fichero.WriteLine("#ValorGuia#" + lstguia(0).total.ToString)
    '    Fichero.WriteLine("#Correlativo#" + GuiaImpresion.ToString)
    '    Fichero.WriteLine("#FechaEmision#" + dt.Rows(0).Item("fecha_Fact").ToString)
    '    'Fichero.WriteLine("#NumeroBoleto#" + PadL(lstguia(0).idguia.ToString + Aleatorio.ToString, 8, "0"))
    '    'Fichero.WriteLine("#Origen#GT")
    '    'Fichero.WriteLine("#FechaViaje#-")
    '    'Fichero.WriteLine("#HoraViaje#-")
    '    'Fichero.WriteLine("#Butaca#-")
    '    'Fichero.WriteLine("#Pasajero#-")
    '    'Fichero.WriteLine("#Destino#HUE")
    '    'Fichero.WriteLine("#TipoServicio#" + lstguia(0).servicio)
    '    Dim str1, str2, cadena As String
    '    str1 = ""
    '    str2 = ""
    '    cadena = dt.Rows(0).Item("Cae").ToString
    '    For n As Integer = 0 To cadena.Length - 1
    '        If n <= 31 Then
    '            str1 = str1 + cadena.Chars(n).ToString
    '        End If
    '        If n > 31 Then
    '            str2 = str2 + cadena.Chars(n).ToString
    '        End If
    '    Next
    '    Fichero.WriteLine("#Cae1#" + str1)
    '    Fichero.WriteLine("#Cae2#" + str2)
    '    Fichero.WriteLine("#NitCliente#" + dt.Rows(0).Item("nit_clt").ToString)
    '    Fichero.WriteLine("#NombreCliente#" + dt.Rows(0).Item("razon_social").ToString)
    '    Fichero.WriteLine("#DireccionCliente#" + dt.Rows(0).Item("dir").ToString)
    '    Fichero.WriteLine("#TelefonoCliente#" + dt.Rows(0).Item("tel").ToString)
    '    'Fichero.WriteLine("#Remitente#" + lstremitente(0).nombreremitente)
    '    'Fichero.WriteLine("#DireccionRemitente#" + lstremitente(0).direccionremitente)
    '    'Fichero.WriteLine("#TelefonoRemitente#" + lstremitente(0).telefonoremitente)
    '    'Fichero.WriteLine("#Destinatario#" + lstdestinatario(0).nombredestinatario)
    '    'Fichero.WriteLine("#TelefonoDestinatario#" + lstdestinatario(0).telefonodestinatario)
    '    'Fichero.WriteLine("#DireccionDestinatario#" + lstdestinatario(0).direcciondestinatario)
    '    For i As Integer = 0 To dt2.Rows.Count - 1
    '        Fichero.WriteLine("#Cantidad" + (i + 1).ToString + "#" + dt2.Rows(i).Item("Cantidad").ToString)
    '        Fichero.WriteLine("#Descripcion" + (i + 1).ToString + "#" + dt2.Rows(i).Item("Artic").ToString)
    '        Fichero.WriteLine("#Valor" + (i + 1).ToString + "#" + dt2.Rows(i).Item("subTotal").ToString)
    '    Next
    '    Fichero.WriteLine("#Total#" + dt.Rows(0).Item("Total").ToString)
    '    'Fichero.WriteLine("#NumeroAsiento#-")
    '    'Fichero.WriteLine("#Servicio#MORALES  ESPECIAL")
    '    'Fichero.WriteLine("#Reimpresion#")
    '    Fichero.WriteLine("#DTE#" + dt.Rows(0).Item("Firma").ToString)
    '    Fichero.WriteLine("#AtendidoPor#" + nemp(0).ToString)
    '    'Fichero.WriteLine("#Origen#" + NemonicoOrigen)
    '    'Fichero.WriteLine("#NombreOrigen#--")
    '    'Fichero.WriteLine("#OrigenAgencia#CAPITAL")
    '    'Fichero.WriteLine("#Destino#" + NemonicoDestino)
    '    'Fichero.WriteLine("#NombreDestino#--")
    '    'Fichero.WriteLine("#DestinoAgencia#HUE")
    '    'Fichero.WriteLine("#DestinoAgenciaNombre#Agencia Huehuetenango")
    '    Fichero.WriteLine(" ")
    '    Fichero.Close()
    '    Return "downloadfiles.aspx?archivo=" & ArchivoPos & ""

    'End Function



    Public Shared Function encabezadoPartidaContable(ByVal tipoDocumento As String, ByVal idDocumento As Integer) As String

        Dim retorno As String = Nothing

        Dim StrInsert As String = "INSERT INTO ENC_PARTIDA " &
                                  "(tipoDocumento, idDocumento) " &
                                  "VALUES " &
                                  "('" & tipoDocumento & "', " & idDocumento & ")"

        Dim realizado As Boolean = EjecutaTransaccion1(StrInsert)

        retorno = idempresabusca("SELECT idPartida FROM ENC_PARTIDA WHERE tipoDocumento = '" & tipoDocumento & "' AND idDocumento = " & idDocumento)

        Return retorno

    End Function

    Public Shared Function detallePartidaContable(ByVal idPartida As Integer, ByVal cuenta As Integer, ByVal debe As Double, ByVal haber As Double) As String

        Dim retorno As String = Nothing

        Dim StrInsert As String = "INSERT INTO DET_PARTIDA " &
                                  "(idPartida, idCuenta, debe, haber) " &
                                  "VALUES " &
                                  "(" & idPartida & ", (SELECT idNivel5 FROM NOMENCLATURA_NIVEL5 WHERE Cuenta = " & cuenta & "), " & debe & ", " & haber & ")"
        retorno = EjecutaTransaccion1(StrInsert)

        Return retorno

    End Function

    Public Shared Function mesNombre(ByVal mes As Integer) As String

        Dim retorno As String = Nothing

        If mes = 1 Then
            retorno = "Enero"
        ElseIf mes = 2 Then
            retorno = "Febrero"
        ElseIf mes = 3 Then
            retorno = "Marzo"
        ElseIf mes = 4 Then
            retorno = "Abril"
        ElseIf mes = 5 Then
            retorno = "Mayo"
        ElseIf mes = 6 Then
            retorno = "Junio"
        ElseIf mes = 7 Then
            retorno = "Julio"
        ElseIf mes = 8 Then
            retorno = "Agosto"
        ElseIf mes = 9 Then
            retorno = "Septiembre"
        ElseIf mes = 10 Then
            retorno = "Octubre"
        ElseIf mes = 11 Then
            retorno = "Noviembre"
        ElseIf mes = 12 Then
            retorno = "Diciembre"
        End If

        Return retorno

    End Function

    Public Shared Function encabezadoPrePartidaContable(ByVal tipoDocumento As String, ByVal idDocumento As Integer) As String

        Dim retorno As String = Nothing

        Dim StrInsert As String = "INSERT INTO ENC_PRE_PARTIDA " &
                                  "(tipoDocumento, idDocumento) " &
                                  "VALUES " &
                                  "('" & tipoDocumento & "', " & idDocumento & ")"

        Dim realizado As Boolean = EjecutaTransaccion1(StrInsert)

        If realizado = True Then
            retorno = idempresabusca("SELECT idPartida FROM ENC_PRE_PARTIDA WHERE tipoDocumento = '" & tipoDocumento & "' AND idDocumento = " & idDocumento)
        Else
            retorno = realizado
        End If

        Return retorno

    End Function

    Public Shared Function detallePrePartidaContable(ByVal idPartida As Integer, ByVal cuenta As Integer, ByVal debe As Double, ByVal haber As Double) As String

        Dim retorno As String = Nothing

        Dim StrInsert As String = "INSERT INTO DET_PRE_PARTIDA " &
                                  "(idPartida, idCuenta, debe, haber) " &
                                  "VALUES " &
                                  "(" & idPartida & ", (SELECT idNivel5 FROM NOMENCLATURA_NIVEL5 WHERE Cuenta = " & cuenta & "), " & debe & ", " & haber & ")"
        retorno = EjecutaTransaccion1(StrInsert)

        Return retorno

    End Function

    Public Shared Function encabezadoPrePartidaContableDiario(ByVal tipoDocumento As String, ByVal idDocumento As String, ByVal usuario As String, ByVal tipoPartida As Integer) As String

        Dim retorno As String = Nothing

        Dim StrInsert As String = "INSERT INTO ENC_PRE_PARTIDA " &
                                  "(tipoDocumento, idDocumento, usuario, tipoPartida) " &
                                  "VALUES " &
                                  "('" & tipoDocumento & "', " & idDocumento & ", '" & usuario & "', " & tipoPartida & ")"

        Dim realizado As Boolean = EjecutaTransaccion1(StrInsert)

        If realizado = True Then
            retorno = idempresabusca("SELECT MAX(idPartida) FROM ENC_PRE_PARTIDA WHERE usuario = '" & usuario & "' AND tipoPartida = " & tipoPartida)
        Else
            retorno = realizado
        End If

        Return retorno

    End Function

    Public Function Datos(ByVal Cons As String)
        cn.Open()
        Try
            Dim cmd As New SqlCommand(Cons, cn)
            Dim da As New SqlDataAdapter(cmd)
            Dim dt As New DataTable
            da.Fill(dt)
            Return dt
        Catch ex As Exception
            'MsgBox(ex.Message)
        Finally
            Me.fin()
        End Try
        Return 0
    End Function

    Private Sub fin()
        Try
            cn.Close()
        Catch ex As Exception
            'MsgBox(ex.Message)
        End Try
    End Sub

    Public Class encabezadoFactura
        Public nitClt As String
        Public nit As String
        Public serie As String
        Public fecha As String
        Public total As Double
        Public descuento As Double
        Public iva As Double
        Public direccion As String
        Public Autorizacion As String
        Public Nom_Muni As String
        Public Cliente As String
        Public Nom_Dep As String
        Public razon As String
        Public Empresa As String
        Public Dire_Cac_Pdv As String
        Public FAutoriza As String
        Public sinIva As Double
        Public EST As Integer
    End Class

    Public Class detalleFactura
        Public Cantidad As Integer
        Public ID_A As String
        Public Artic As String
        Public sinIva As Double
        Public Precio As Double
        Public subTotal As Double
        Public Descuento As Double
        Public Iva As Double
        Public ProServ As String
    End Class




    Public Shared Function MailEnvio(ByVal strsql As String) As String
        Dim cn As New SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("ConString").ConnectionString)
        cn.Open()
        Dim Resultado1 As String
        Try
            Dim comando As New SqlCommand
            Dim transaccion As SqlTransaction
            transaccion = cn.BeginTransaction
            comando.Connection = cn
            comando.Transaction = transaccion

            comando.CommandText = strsql
            Resultado1 = Convert.ToString(comando.ExecuteScalar)

        Catch ex As Exception
            ' MsgBox(ex.Message)
        Finally
            cn.Close()
        End Try
    End Function

End Class

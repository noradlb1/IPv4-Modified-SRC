Imports System.Net.NetworkInformation
Imports System.Net
Imports System.Management

Public Class Form1

    Public Sub DisplayDnsConfiguration()
        Try
            For Each ni As NetworkInterface In NetworkInterface.GetAllNetworkInterfaces()
                If ni.NetworkInterfaceType = NetworkInterfaceType.Wireless80211 OrElse ni.NetworkInterfaceType = NetworkInterfaceType.Ethernet Then
                    ComboBox1.Items.Add(ni.Description)
                End If
            Next
        Catch ex As Exception
        End Try


    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles Me.Load
        Try
           
                With ComboBox1
                    .DropDownStyle = ComboBoxStyle.DropDownList
                    DisplayDnsConfiguration()


            End With
            With ComboBox2
                .DropDownStyle = ComboBoxStyle.DropDownList
                .Items.Add(" لتسريع التصفـــح")
                .Items.Add(" الحماية والتخفي")
                .Items.Add(" حجب المواقع الإباحية")
                .Items.Add(" حجب المواقع الإباحية")
                .Items.Add("حجب المواقع الإباحية")
                .Items.Add(" حجب المواقع الإباحية")

            End With
        Catch ex As Exception

        End Try

    End Sub

    Private Sub ComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox1.SelectedIndexChanged
        Try
            Dim r As New Random
            Dim dtext As String = TextBox4.Text + TextBox5.Text
            Dim wmi As ManagementClass
            Dim obj As ManagementObject
            Dim objs As ManagementObjectCollection
            For Each nic As NetworkInterface In NetworkInterface.GetAllNetworkInterfaces()
                For Each gatewayAddr In nic.GetIPProperties().GatewayAddresses
                    For Each dnsaddr In nic.GetIPProperties().DnsAddresses
                        For Each ip As UnicastIPAddressInformation In nic.GetIPProperties().UnicastAddresses
                            If nic Is Nothing OrElse nic.NetworkInterfaceType < 1 Then
                                MsgBox("No network found in this Pc ....!")
                            End If
                            If nic.Description = ComboBox1.SelectedItem.ToString() Then
                                If ip.Address.AddressFamily = System.Net.Sockets.AddressFamily.InterNetwork Then
                                    TextBox10.Text = (ip.Address.ToString())
                                    TextBox1.Text = (ip.Address.ToString())
                                    TextBox3.Text = (gatewayAddr.Address.ToString)
                                    TextBox8.Text = (gatewayAddr.Address.ToString)
                                    TextBox2.Text = ("255.255.255.0")
                                    TextBox9.Text = ("255.255.255.0")
                                    wmi = New ManagementClass("Win32_NetworkAdapterConfiguration")
                                    objs = wmi.GetInstances()
                                    For Each obj In objs
                                        If Not IsNothing(obj("DNSServerSearchOrder")) AndAlso UBound(obj("DNSServerSearchOrder")) >= 0 Then
                                            TextBox4.Text = (obj("DNSServerSearchOrder")(0))
                                            TextBox7.Text = (obj("DNSServerSearchOrder")(0))
                                            TextBox11.Text = TextBox4.Text & "," & TextBox5.Text
                                        End If
                                        If Not IsNothing(obj("DNSServerSearchOrder")) AndAlso UBound(obj("DNSServerSearchOrder")) >= 1 Then
                                            TextBox5.Text = (obj("DNSServerSearchOrder")(1))
                                            TextBox6.Text = (obj("DNSServerSearchOrder")(1))
                                            TextBox12.Text = TextBox7.Text & "," & TextBox6.Text
                                        End If
                                        Try
                                            If TextBox1.Text.Contains("192.168.1.") Then
                                                TextBox1.Text = (("192.168.1." & r.Next(0, 255)))
                                            End If
                                            If ComboBox2.SelectedIndex < 0 Then
                                                TextBox4.Text = ""
                                            End If
                                            If TextBox12.Text = "8.8.8.8,8.8.4.4" Then
                                                ComboBox2.SelectedIndex = 0
                                            ElseIf TextBox12.Text = "87.216.170.8,185.16.40.143" Then
                                                ComboBox2.SelectedIndex = 1
                                            ElseIf TextBox12.Text = "208.67.222.123,208.67.220.123" Then
                                                ComboBox2.SelectedIndex = 2
                                            ElseIf TextBox12.Text = "198.153.192.50,198.153.194.50" Then
                                                ComboBox2.SelectedIndex = 3
                                            ElseIf TextBox12.Text = "77.88.8.7,77.88.8.3" Then
                                                ComboBox2.SelectedIndex = 4
                                            ElseIf TextBox12.Text = "180.131.144.144,180.131.144.145" Then
                                                ComboBox2.SelectedIndex = 5
                                            End If
                                        Catch ex As Exception

                                        End Try
                                    Next

                                    objs.Dispose()
                                    wmi.Dispose()
                                End If
                            End If
                        Next
                    Next
                Next
            Next
        Catch ex As Exception

        End Try

    End Sub
   

   
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Try
            If ComboBox1.SelectedIndex < 0 Then
                MsgBox("Please Select your Network Adapter !!", vbInformation, "IPv4 Modified By Professor Nahrawan")
                Exit Sub
            End If
            Dim IPAddress As String = TextBox1.Text
            Dim SubnetMask As String = TextBox2.Text
            Dim Gateway As String = TextBox3.Text
            Dim DNS1 As String = TextBox11.Text


            Dim objMC As ManagementClass = New ManagementClass("Win32_NetworkAdapterConfiguration")
            Dim objMOC As ManagementObjectCollection = objMC.GetInstances()

            For Each objMO As ManagementObject In objMOC
                If (Not CBool(objMO("IPEnabled"))) Then
                    Continue For
                End If

                Try
                    Dim objNewIP As ManagementBaseObject = Nothing
                    Dim objSetIP As ManagementBaseObject = Nothing
                    Dim objNewGate As ManagementBaseObject = Nothing

                    objNewIP = objMO.GetMethodParameters("EnableStatic")
                    objNewGate = objMO.GetMethodParameters("SetGateways")

                    'Set DefaultGateway
                    objNewGate("DefaultIPGateway") = New String() {Gateway}
                    objNewGate("GatewayCostMetric") = New Integer() {1}

                    'Set IPAddress and Subnet Mask
                    objNewIP("IPAddress") = New String() {IPAddress}
                    objNewIP("SubnetMask") = New String() {SubnetMask}

                    objSetIP = objMO.InvokeMethod("EnableStatic", objNewIP, Nothing)
                    objSetIP = objMO.InvokeMethod("SetGateways", objNewGate, Nothing)


                    Dim objNewDNS As ManagementBaseObject = objMO.GetMethodParameters("SetDNSServerSearchOrder")

                    'Set DNS to DHCP
                    objNewDNS("DNSServerSearchOrder") = DNS1.Split(","c)
                    objMO.InvokeMethod("SetDNSServerSearchOrder", objNewDNS, Nothing)

                    MsgBox("Done !!!IPv4 Modified By Professor Nahrawan", vbInformation, "IPv4 Modified By Professor Nahrawan")

                Catch ex As Exception
                    MessageBox.Show("Unable to Set IP : " & ex.Message)
                End Try
            Next objMO
        Catch ex As Exception

        End Try

    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Try
            Dim r As New Random

            If TextBox1.Text.Contains("192.168.1.") Then
                TextBox1.Text = (("192.168.1." & r.Next(0, 255)))
            End If

        Catch ex As Exception

        End Try
    End Sub
   
    Private Sub ComboBox2_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox2.SelectedIndexChanged
        Try
            If ComboBox2.SelectedIndex = 0 Then
                TextBox11.Text = "8.8.8.8,8.8.4.4"
            ElseIf ComboBox2.SelectedIndex = 1 Then
                TextBox11.Text = "87.216.170.8,185.16.40.143"
            ElseIf ComboBox2.SelectedIndex = 2 Then
                TextBox11.Text = "208.67.222.123,208.67.220.123"
            ElseIf ComboBox2.SelectedIndex = 3 Then
                TextBox11.Text = "198.153.192.50,198.153.194.50"
            ElseIf ComboBox2.SelectedIndex = 4 Then
                TextBox11.Text = "77.88.8.7,77.88.8.3"
            ElseIf ComboBox2.SelectedIndex = 5 Then
                TextBox11.Text = "180.131.144.144,180.131.144.145"
            End If
           

        Catch ex As Exception

        End Try

    End Sub


   
  
  
End Class

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RM.Model
{
    public partial class frmBillList : SampleAdd
    {
        public frmBillList()
        {
            InitializeComponent();
        }
        public int MainID = 0;
        private void frmBillList_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        private void LoadData()
        {
            string qry = @"select MainID ,TableName,WaiterName,orderType,status,total from tblMain
                           where status <> 'Pending' ";
            ListBox lb = new ListBox();
            lb.Items.Add(dgvid);
            lb.Items.Add(dgvtable);
            lb.Items.Add(dgvWaiter);
            lb.Items.Add(dgvType);
            lb.Items.Add(dgvStatus);
            lb.Items.Add(dgvTotal);

            MainClass.LoadData(qry, guna2DataGridView1, lb);
        }

        private void guna2DataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            int count = 0;

            foreach (DataGridViewRow row in guna2DataGridView1.Rows)
            {
                count++;
                row.Cells[0].Value = count;
            }
        }

        private void guna2DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (guna2DataGridView1.CurrentCell.OwningColumn.Name == "dgvedit")
            {
                
                MainID = Convert.ToInt32(guna2DataGridView1.CurrentRow.Cells["dgvid"].Value);
                this.Close();
                
            }
            else if (guna2DataGridView1.CurrentCell.OwningColumn.Name == "dgvbel")
            {
                // Retrieve MainID of the selected row
                MainID = Convert.ToInt32(guna2DataGridView1.CurrentRow.Cells["dgvid"].Value);

                // Confirm deletion with the user
                DialogResult result = MessageBox.Show("Are you sure you want to delete this bill?", "Delete Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    // Delete the row from the database
                    using (SqlConnection conn = new SqlConnection("Data Source=DESKTOP-DGHUL2O; Initial Catalog=RM; Integrated Security=True;"))
                    {
                        string deleteQuery = "DELETE FROM tblMain WHERE MainID = @MainID; DELETE FROM tblDetails WHERE MainID = @MainID;";
                        using (SqlCommand cmd = new SqlCommand(deleteQuery, conn))
                        {
                            cmd.Parameters.AddWithValue("@MainID", MainID);
                            conn.Open();
                            cmd.ExecuteNonQuery();
                            conn.Close();
                        }
                    }

                    // Remove the row from the DataGridView
                    guna2DataGridView1.Rows.RemoveAt(e.RowIndex);

                    // Optionally, show a success message
                    MessageBox.Show("Bill deleted successfully.", "Deletion Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else if (guna2DataGridView1.CurrentCell.OwningColumn.Name == "dgvdel")
            {
                // Retrieve MainID of the selected bill
                MainID = Convert.ToInt32(guna2DataGridView1.CurrentRow.Cells["dgvid"].Value);

                // Call the method to print the bill
                PrintBill();
            }


        }
        private void PrintBill()
        {
            PrintDocument printDoc = new PrintDocument();
            printDoc.PrintPage += new PrintPageEventHandler(PrintPage);
            printDoc.DefaultPageSettings.PaperSize = new PaperSize("pprnm", 285, 600);
            printDoc.Print();
        }

        private void PrintPage(object sender, PrintPageEventArgs e)
        {
            string qry = @"SELECT * FROM tblMain m 
                   INNER JOIN tblDetails d ON d.MainID = m.MainID 
                   INNER JOIN products p ON p.PID = d.ProID
                   WHERE m.MainID = @MainID";

            using (SqlConnection conn = new SqlConnection("Data Source=DESKTOP-DGHUL2O; Initial Catalog=RM; Integrated Security=True;"))
            using (SqlCommand cmd = new SqlCommand(qry, conn))
            {
                cmd.Parameters.AddWithValue("@MainID", MainID);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                int yPosition = 20;
                int lineHeight = 28;
                Font printFont = new Font("Arial", 10);

                // Print headers
                e.Graphics.DrawString("Receipt", new Font("Arial", 18, FontStyle.Bold), Brushes.Black, new PointF(300, yPosition));
                yPosition += lineHeight;
                e.Graphics.DrawString("Macmin Restaurant", new Font("Arial", 14, FontStyle.Bold), Brushes.Black, new PointF(260, yPosition));
                yPosition += lineHeight;
                e.Graphics.DrawString("--------------------------------------------------------------------------------------------------------------------------------------", printFont, Brushes.Black, new PointF(10, yPosition));
                yPosition += lineHeight;

                // Check for data and move to the first row to print basic details
                if (reader.HasRows && reader.Read())
                {
                    // Retrieve the total, received, and change values beforehand
                    string total = reader["total"].ToString();
                    string received = reader["received"].ToString();
                    string change = reader["change"].ToString();

                    // Print order details
                    e.Graphics.DrawString("Date: " + reader["aDate"].ToString(), printFont, Brushes.Black, new PointF(10, yPosition));
                    yPosition += lineHeight;
                    e.Graphics.DrawString("Time: " + reader["aTime"].ToString(), printFont, Brushes.Black, new PointF(10, yPosition));
                    yPosition += lineHeight;
                    e.Graphics.DrawString("Customer Name: " + reader["CustName"].ToString(), printFont, Brushes.Black, new PointF(10, yPosition));
                    yPosition += lineHeight;
                    e.Graphics.DrawString("Customer Phone: " + reader["CustPhone"].ToString(), printFont, Brushes.Black, new PointF(10, yPosition));
                    yPosition += lineHeight;
                    e.Graphics.DrawString("Table: " + reader["TableName"].ToString(), printFont, Brushes.Black, new PointF(10, yPosition));
                    yPosition += lineHeight;
                    e.Graphics.DrawString("Waiter: " + reader["WaiterName"].ToString(), printFont, Brushes.Black, new PointF(10, yPosition));
                    yPosition += lineHeight;
                    e.Graphics.DrawString("Order Type: " + reader["orderType"].ToString(), printFont, Brushes.Black, new PointF(10, yPosition));
                    yPosition += lineHeight;
                    e.Graphics.DrawString("Status: " + reader["status"].ToString(), printFont, Brushes.Black, new PointF(10, yPosition));
                    yPosition += lineHeight;

                    // Separator line
                    e.Graphics.DrawString("--------------------------------------------------------------------------------------------------------------------------------------", printFont, Brushes.Black, new PointF(10, yPosition));
                    yPosition += lineHeight;

                    // Column headers for products
                    e.Graphics.DrawString("Product ID", new Font("Arial", 10, FontStyle.Bold), Brushes.Black, new PointF(10, yPosition));
                    e.Graphics.DrawString("Product Name", new Font("Arial", 10, FontStyle.Bold), Brushes.Black, new PointF(150, yPosition));
                    e.Graphics.DrawString("Qty", new Font("Arial", 10, FontStyle.Bold), Brushes.Black, new PointF(370, yPosition));
                    e.Graphics.DrawString("Price", new Font("Arial", 10, FontStyle.Bold), Brushes.Black, new PointF(450, yPosition));
                    e.Graphics.DrawString("Amount", new Font("Arial", 10, FontStyle.Bold), Brushes.Black, new PointF(530, yPosition));
                    yPosition += lineHeight;

                    // Loop to print each product line item
                    do
                    {
                        e.Graphics.DrawString(reader["proID"].ToString(), printFont, Brushes.Black, new PointF(10, yPosition));
                        e.Graphics.DrawString(reader["pName"].ToString(), printFont, Brushes.Black, new PointF(150, yPosition));
                        e.Graphics.DrawString(reader["qty"].ToString(), printFont, Brushes.Black, new PointF(370, yPosition));
                        e.Graphics.DrawString(reader["price"].ToString(), printFont, Brushes.Black, new PointF(450, yPosition));
                        e.Graphics.DrawString(reader["amount"].ToString(), printFont, Brushes.Black, new PointF(530, yPosition));
                        yPosition += lineHeight;

                    } while (reader.Read());

                    // Separator line for totals
                    e.Graphics.DrawString("--------------------------------------------------------------------------------------------------------------------------------------", printFont, Brushes.Black, new PointF(10, yPosition));
                    yPosition += lineHeight;

                    // Print total, cash received, and change
                    e.Graphics.DrawString("Total: ", new Font("Arial", 10, FontStyle.Bold), Brushes.Black, new PointF(400, yPosition));
                    e.Graphics.DrawString(total, new Font("Arial", 10, FontStyle.Bold), Brushes.Black, new PointF(530, yPosition));
                    yPosition += lineHeight;
                    e.Graphics.DrawString("Cash Received:", new Font("Arial", 10, FontStyle.Bold), Brushes.Black, new PointF(400, yPosition));
                    e.Graphics.DrawString(received, new Font("Arial", 10, FontStyle.Bold), Brushes.Black, new PointF(530, yPosition));
                    yPosition += lineHeight;
                    e.Graphics.DrawString("Change:", new Font("Arial", 10, FontStyle.Bold), Brushes.Black, new PointF(400, yPosition));
                    e.Graphics.DrawString(change, new Font("Arial", 10, FontStyle.Bold), Brushes.Black, new PointF(530, yPosition));
                    yPosition += lineHeight;

                    e.Graphics.DrawString("--------------------------------------------------------------------------------------------------------------------------------------", printFont, Brushes.Black, new PointF(10, yPosition));
                    yPosition += lineHeight;
                    e.Graphics.DrawString("--------------------------------------------------------------------------------------------------------------------------------------", printFont, Brushes.Black, new PointF(10, yPosition));
                    yPosition += lineHeight;
                }

                else
                {
                    e.Graphics.DrawString("No data found for this bill.", printFont, Brushes.Black, new PointF(10, yPosition));
                }
            }
        }


    }
    
}

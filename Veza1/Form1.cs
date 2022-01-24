using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;

namespace Veza1
{
    public partial class Form1 : Form
    {

        private SQLiteConnection sqlite_conn; //для соединения с базой
        private string baseName = "ToolsBase.db3"; //имя базы
        AutoCompleteStringCollection ac_source = new AutoCompleteStringCollection(); //переменная для автозаполнения

        public Form1()
        {
            InitializeComponent();
            
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void add_new_tool(object sender, EventArgs e) //кнопка ДОБАВИТЬ
        {
            if (textbox_name_add.Text != "" && textbox_mark_add.Text != "" && textbox_description_add.Text != "" && comboBox1.Text != "" && comboBox2.Text != "")
            {
                sqlite_conn = new SQLiteConnection("Data Source=ToolsBase.db3;Version=3;");
                sqlite_conn.Open();
                //SQLiteCommand table_add = new SQLiteCommand("INSERT INTO Oborudovanie(name, mark, description, category, branch) VALUES ('" + textbox_name_add.Text + "', '" + textbox_mark_add.Text + "', '" + textbox_description_add.Text + "', '" + comboBox1.Text + "', '" + comboBox2.Text + "')", sqlite_conn);
                //альтернатива ниже
                SQLiteCommand table_add = sqlite_conn.CreateCommand();
                table_add.CommandText = "INSERT INTO Oborudovanie(name, mark, description, category, branch) VALUES (@name_add, @mark_add, @description_add, @category_add, @branch_add)";
                table_add.Parameters.Add("@name_add", System.Data.DbType.String).Value = textbox_name_add.Text;
                table_add.Parameters.Add("@mark_add", System.Data.DbType.String).Value = textbox_mark_add.Text;
                table_add.Parameters.Add("@description_add", System.Data.DbType.String).Value = textbox_description_add.Text;
                table_add.Parameters.Add("@category_add", System.Data.DbType.String).Value = comboBox1.Text;
                table_add.Parameters.Add("@branch_add", System.Data.DbType.String).Value = comboBox2.Text;
                table_add.ExecuteNonQuery();
                sqlite_conn.Close();
                label_info.Text = "Оборудование успешно добавлено в базу!";
                textbox_name_add.Text = "";
                textbox_mark_add.Text = "";
                textbox_description_add.Text = "";
                load_grid();
                ac_source_update(ac_source);

            } else
            {
                label_info.Text = "Не все поля заполнены!";
            }
        }


        private void Form1_Load(object sender, EventArgs e) //загрузка формы
        {
            textbox_mark_update.ReadOnly = true;
            textbox_description_update.ReadOnly = true;
            textbox_category_update.ReadOnly = true;
            textbox_branch_update.ReadOnly = true;

            comboBox5.Text = comboBox5.Items[0].ToString();
            comboBox6.Text = comboBox6.Items[0].ToString();



            if (!System.IO.File.Exists(baseName)) //если базы не существует - создать основу
            {
                //var formPopup = new Form();
                //formPopup.Show(this);
                SQLiteConnection.CreateFile(baseName); //создаём базу
                sqlite_conn = new SQLiteConnection("Data Source=ToolsBase.db3;Version=3;New=False;Compress=True;");
                sqlite_conn.Open();
                SQLiteCommand table_create = new SQLiteCommand("CREATE TABLE IF NOT EXISTS [Oborudovanie]([id] INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL UNIQUE, [name] TEXT, [mark] TEXT, [description] TEXT, [category] TEXT, [branch] TEXT);", sqlite_conn);
                table_create.ExecuteNonQuery();
                sqlite_conn.Close();
            }
            load_grid();
            ac_source_update(ac_source); //заполняем сурс автокомплита
            


        }

        private void button_cancel(object sender, EventArgs e)
        {
            textbox_name_add.Text = "";
            textbox_mark_add.Text = "";
            textbox_description_add.Text = "";
            comboBox1.Text = "";
            comboBox2.Text = "";
            label_info.Text = "Поля ввода очищены!";
        }

        private void button_refresh_Click(object sender, EventArgs e)
        {
            load_grid();

            
        }
        private void load_grid() //загрузка таблицы из базы
        {
            sqlite_conn = new SQLiteConnection("Data Source=ToolsBase.db3;Version=3;");
            sqlite_conn.Open();
            SQLiteCommand db_show = new SQLiteCommand("SELECT * FROM Oborudovanie;", sqlite_conn);
            SQLiteDataReader db_reader = db_show.ExecuteReader();
            List<String[]> data = new List<String[]>();
            data.Clear();
            data_grid.Rows.Clear();


            while (db_reader.Read())
            {
                data.Add(new String[6]);
                data[data.Count - 1][0] = db_reader[0].ToString();
                data[data.Count - 1][1] = db_reader[1].ToString();
                data[data.Count - 1][2] = db_reader[2].ToString();
                data[data.Count - 1][3] = db_reader[3].ToString();
                data[data.Count - 1][4] = db_reader[4].ToString();
                data[data.Count - 1][5] = db_reader[5].ToString();
            }
            db_reader.Close();
            sqlite_conn.Close();

            foreach (string[] s in data)
            {
                data_grid.Rows.Add(s);
            }
        }

        private void textbox_ac_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void textbox_ac_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                sqlite_conn = new SQLiteConnection("Data Source=ToolsBase.db3;Version=3;");
                sqlite_conn.Open();
                SQLiteCommand name_show = new SQLiteCommand("SELECT * FROM Oborudovanie WHERE name = '" + textbox_ac.Text + "'", sqlite_conn);
                SQLiteDataReader db_reader = name_show.ExecuteReader();
                while (db_reader.Read())   // построчно считываем данные
                {
                    label_id_update.Text = db_reader.GetValue(0).ToString();
                    textbox_mark_update.Text = db_reader.GetValue(2).ToString();
                    textbox_description_update.Text = db_reader.GetValue(3).ToString();
                    textbox_category_update.Text = db_reader.GetValue(4).ToString();
                    textbox_branch_update.Text = db_reader.GetValue(5).ToString();

                }
                if (textbox_mark_update.Text != "")
                {
                    textbox_mark_update.ReadOnly = false;
                }
                if (textbox_description_update.Text != "")
                {
                    textbox_description_update.ReadOnly = false;
                }
                if (textbox_category_update.Text != "")
                {
                    textbox_category_update.ReadOnly = false;
                }
                if (textbox_branch_update.Text != "")
                {
                    textbox_branch_update.ReadOnly = false;
                }
                sqlite_conn.Close();
                
            }
        }

        private void button3_Click(object sender, EventArgs e) //кнопка отмены в редактировании
        {
            butt_deactivate();
        }
        private void ac_source_update(AutoCompleteStringCollection ac_s) //автокомплит
        {
            ac_s.Clear();
            sqlite_conn = new SQLiteConnection("Data Source=ToolsBase.db3;Version=3;New=False;Compress=True;");
            sqlite_conn.Open();
            SQLiteCommand select_name = new SQLiteCommand("SELECT name FROM Oborudovanie;", sqlite_conn);
            SQLiteDataReader db_reader = select_name.ExecuteReader();
            while (db_reader.Read())
            {
                ac_s.Add(db_reader[0].ToString());
            }
            db_reader.Close();
            sqlite_conn.Close();
            textbox_ac.AutoCompleteCustomSource = ac_s;
            textbox_ac.AutoCompleteSource = AutoCompleteSource.CustomSource;
            textbox_ac.AutoCompleteMode = AutoCompleteMode.Append;
        }

        private void button4_Click(object sender, EventArgs e) //редактирование
        {
            sqlite_conn = new SQLiteConnection("Data Source=ToolsBase.db3;Version=3;");
            sqlite_conn.Open();
            SQLiteCommand table_change = new SQLiteCommand("UPDATE Oborudovanie SET name = '" + textbox_ac.Text + "', mark = '" + textbox_mark_update.Text + "', description = '" + textbox_description_update.Text + "', category = '" + textbox_category_update.Text + "', branch = '" + textbox_branch_update.Text + "' WHERE id = " + label_id_update.Text + ";", sqlite_conn);
            table_change.ExecuteNonQuery();
            sqlite_conn.Close();
            load_grid();
            ac_source_update(ac_source);
            butt_deactivate();
        }

        private void button5_Click(object sender, EventArgs e) //удаляет оборудование
        {
            sqlite_conn = new SQLiteConnection("Data Source=ToolsBase.db3;Version=3;");
            sqlite_conn.Open();
            //SQLiteCommand table_delete = new SQLiteCommand("DELETE FROM Oborudovanie WHERE id ="+ label_id_update.Text +"AND name = "+ textbox_ac.Text + ";");
            SQLiteCommand table_delete = new SQLiteCommand("DELETE FROM Oborudovanie WHERE id = "+label_id_update.Text+" AND name = '"+ textbox_ac.Text + "';", sqlite_conn);
            table_delete.ExecuteNonQuery();
            sqlite_conn.Close();
            load_grid();
            ac_source_update(ac_source);
            butt_deactivate();
        }
        private void butt_deactivate()
        {
            label_id_update.Text = "none";
            textbox_ac.Text = "";
            textbox_mark_update.Text = "";
            textbox_description_update.Text = "";
            textbox_category_update.Text = "";
            textbox_branch_update.Text = "";
            textbox_mark_update.ReadOnly = true;
            textbox_description_update.ReadOnly = true;
            textbox_category_update.ReadOnly = true;
            textbox_branch_update.ReadOnly = true;
        }

        private void button6_Click(object sender, EventArgs e) //показывает список по выборке
        {
            if (checkBox1.Checked == false && checkBox2.Checked == false)
            {
                label_check.Text = "Ничего не выбрано!";
            }
            else
            {
                String comm_st = "SELECT * FROM Oborudovanie WHERE ";
                sqlite_conn = new SQLiteConnection("Data Source=ToolsBase.db3;Version=3;");
                sqlite_conn.Open();
                if (checkBox1.Checked == true && comboBox5.Text != "")
                {
                    comm_st += "category = '" + comboBox5.Text + "'";
                }
                if (checkBox1.Checked == true && checkBox2.Checked == true)
                {
                    comm_st += "AND ";
                }
                if (checkBox2.Checked == true && comboBox6.Text != "")
                {
                    comm_st += " branch = '" + comboBox6.Text + "'";
                }
                comm_st += ";";
                SQLiteCommand select_category = new SQLiteCommand(comm_st, sqlite_conn);
                SQLiteDataReader db_reader = select_category.ExecuteReader();
                List<String[]> data = new List<String[]>();
                data.Clear();
                data_grid.Rows.Clear();


                while (db_reader.Read())
                {
                    data.Add(new String[6]);
                    data[data.Count - 1][0] = db_reader[0].ToString();
                    data[data.Count - 1][1] = db_reader[1].ToString();
                    data[data.Count - 1][2] = db_reader[2].ToString();
                    data[data.Count - 1][3] = db_reader[3].ToString();
                    data[data.Count - 1][4] = db_reader[4].ToString();
                    data[data.Count - 1][5] = db_reader[5].ToString();
                }
                db_reader.Close();
                sqlite_conn.Close();
                foreach (string[] s in data)
                {
                    data_grid.Rows.Add(s);
                }
                label_check.Text = "Данные выведены";
            }
        }
    }
        

}



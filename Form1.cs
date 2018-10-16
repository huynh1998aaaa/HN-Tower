using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
namespace TowerOfHanoi
{
	public partial class ControlForm : Form
	{
		Stack<PictureBox> RodA, RodB, RodC, FirstSelect, SecondSelect;
		PictureBox[] DISC;
		TimeSpan time;
		Queue<Tuple<PictureBox, PictureBox>> tempQ;
		const int FirstY = 320, DiscHeight = 35, XFormAtoB = 10;
		int move;
		public ControlForm()
		{
			InitializeComponent();
			DISC = new PictureBox[] { Pic1, Pic2, Pic3, Pic4, Pic5, Pic6, Pic7, Pic8, Pic9 };
			ABoxPic.Tag = RodA = new Stack<PictureBox>();
			BBoxPic.Tag = RodB = new Stack<PictureBox>();
			CBoxPic.Tag = RodC = new Stack<PictureBox>();
			FirstSelect = new Stack<PictureBox>();
			SecondSelect = new Stack<PictureBox>();
			tempQ = new Queue<Tuple<PictureBox, PictureBox>>();
		}

		private void pictureBox3_Click(object sender, EventArgs e)
		{

		}

		private void LawButton_Click(object sender, EventArgs e)
		{
			MessageBox.Show("Trò chơi tháp Hà Nội với 3 cọc A, B, C, số đĩa có thể chọn trong khoảng từ 1 đến 9. Nhiệm vụ là hãy di chuyển tất cả các đĩa từ cọc A sang cọc C theo luật:\n  -Mỗi lần chỉ được di chuyển 1 đĩa trên cùng của cọc\n  -Đĩa nằm trên phải nhỏ hơn đĩa nằm dưới", "Luật chơi", MessageBoxButtons.OK, MessageBoxIcon.Information);
		}

		private void timer1_Tick(object sender, EventArgs e)
		{
			time = time.Add(new TimeSpan(0, 0, 1));
			label4.Text = string.Format("Thời gian chơi {0:00}:{1:00}:{2:00}", time.Hours, time.Minutes, time.Seconds);
		}

		private void GiveUpButton_Click(object sender, EventArgs e)
		{
			timer1.Stop();
			timer2.Stop();
			PlayButton.Text = "Chơi";
			Level.Enabled = true;
			GiveUpButton.Enabled = false;
		}

		private void PlayButton_Click(object sender, EventArgs e)
		{
			//reset
			timer1.Stop();
			timer2.Stop();
			tempQ.Clear();
			foreach (PictureBox item in DISC)
			{
				item.Visible = false;
			}
			time = new TimeSpan(0);
			label4.Text = "Thời gian chơi 00:00:00";
			move = 0;
			label5.Text = "Số bước đi : 0";
			label7.Text = "trên 0";
			RodA.Clear(); RodB.Clear(); RodC.Clear();
			ABoxPic.BackColor = BBoxPic.BackColor = CBoxPic.BackColor = System.Drawing.Color.Gray;
			FirstSelect = SecondSelect = null;

			//play
			Level.Enabled = false;
			GiveUpButton.Enabled = true;
			MinMove(Level.Value);
			PlayButton.Text = "Chơi Lại";
			int y = FirstY, x = ABoxPic.Location.X + (9 - (int)Level.Value) * 10;
			for (int i = (int)Level.Value - 1; i >= 0; i--, x += XFormAtoB, y -= DiscHeight)
			{
				DISC[i].Location = new Point(x, y);
				DISC[i].Visible = true;
				RodA.Push(DISC[i]);
			}
			timer1.Start();

		}


		private void MinMove(decimal p)
		{
			switch ((int)p)
			{
				case 1:
					label7.Text = "trên 1";
					break;

				case 2:
					label7.Text = "trên 3";
					break;

				case 3:
					label7.Text = "trên 7";
					break;
				case 4:
					label7.Text = "trên 15";
					break;
				case 5:
					label7.Text = "trên 31";
					break;
				case 6:
					label7.Text = "trên 63";
					break;
				case 7:
					label7.Text = "trên 127";
					break;
				case 8:
					label7.Text = "trên 255";
					break;
				case 9:
					label7.Text = "trên 511";
					break;
				default:
					break;
			}
		}

		private void ABoxPic_Click(object sender, EventArgs e)
		{
			if (Level.Enabled == true)
			{
				return;
			}
			PictureBox ClickedBox = sender as PictureBox;
			Stack<PictureBox> TempStack = (Stack<PictureBox>)ClickedBox.Tag;
			if (FirstSelect == null)
			{
				if (TempStack.Count == 0) return;
				FirstSelect = TempStack;
				ClickedBox.BackColor = System.Drawing.Color.DimGray;
			}
			else if (SecondSelect == null)
			{
				if (TempStack == FirstSelect)
				{
					FirstSelect = null;
					ClickedBox.BackColor = System.Drawing.Color.Gray;
					return;
				}
				SecondSelect = TempStack;
				ProcessMovingDisc(ClickedBox);
			}

		}

		private void ProcessMovingDisc(PictureBox ClickedBox)
		{
			if (SecondSelect.Count == 0)
			{
				PictureBox a = FirstSelect.Peek();
				Moving(new Point(ClickedBox.Location.X + (9 - (int.Parse(a.Tag.ToString()))) * 10, FirstY));
			}
			else
			{
				PictureBox a = FirstSelect.Peek();
				PictureBox b = SecondSelect.Peek();
				if (int.Parse(a.Tag.ToString()) < int.Parse(b.Tag.ToString()))
				{
					Moving(new Point(ClickedBox.Location.X + (9 - (int.Parse(a.Tag.ToString()))) * 10, b.Location.Y - DiscHeight));
				}
				else SecondSelect = null;
			}
		}

		private void Moving(Point point)
		{
			PictureBox a = FirstSelect.Pop();
			a.Location = point;
			SecondSelect.Push(a);
			move++;
			label5.Text = string.Format("Số bước đi : {0}", move);
			FirstSelect = SecondSelect = null;
			ABoxPic.BackColor = BBoxPic.BackColor = CBoxPic.BackColor = System.Drawing.Color.Gray;

			if (RodC.Count == Level.Value && Level.Value != 9)
			{
				GiveUpButton.PerformClick();
				MessageBox.Show("Bạn đã qua màn này. Mời bạn chơi level tiếp theo  ", "CHÚC MỪNG!!!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
				Level.Value = Level.Value + 1;
				PlayButton.PerformClick();
			}
			else if (RodC.Count == Level.Value && Level.Value == 9)
			{
				GiveUpButton.PerformClick();
				MessageBox.Show("Bạn đã vượt qua 9 màn, trò chơi kết thúc  ", "CHÚC MỪNG!!!", MessageBoxButtons.OK, MessageBoxIcon.Stop);


			}//if(Level.Value<8,)
			//throw new NotImplementedException();
		}

		private void Pic1_Click(object sender, EventArgs e)
		{
			PictureBox temp = sender as PictureBox;
			if (RodA.Contains(temp))
			{
				ABoxPic_Click(ABoxPic, new EventArgs());
			}
			if (RodB.Contains(temp))
			{
				ABoxPic_Click(BBoxPic, new EventArgs());
			}
			if (RodC.Contains(temp))
			{
				ABoxPic_Click(CBoxPic, new EventArgs());
			}
		}

		private void button1_Click(object sender, EventArgs e)
		{
			PlayButton.PerformClick();
			ThapHaNoi(Level.Value, ABoxPic, BBoxPic, CBoxPic);
			timer2.Start();
		}

		private void ThapHaNoi(decimal p, PictureBox ABoxPic, PictureBox BBoxPic, PictureBox CBoxPic)
		{
			if (p == 1)
			{
				//ABoxPic_Click(ABoxPic, new EventArgs());
				//ABoxPic_Click(CBoxPic, new EventArgs());
				Tuple<PictureBox, PictureBox> a = new Tuple<PictureBox, PictureBox>(ABoxPic, CBoxPic);
				tempQ.Enqueue(a);
				//Thread.Sleep(1000);
			}
			else
			{
				ThapHaNoi(p - 1, ABoxPic, CBoxPic, BBoxPic);
				//ABoxPic_Click(ABoxPic, new EventArgs());
				//ABoxPic_Click(CBoxPic, new EventArgs());
				Tuple<PictureBox, PictureBox> a = new Tuple<PictureBox, PictureBox>(ABoxPic, CBoxPic);
				tempQ.Enqueue(a);
				//Thread.Sleep(1000);
				ThapHaNoi(p - 1, BBoxPic, ABoxPic, CBoxPic);
			}
		}

		private void timer2_Tick(object sender, EventArgs e)
		{
			if (tempQ.Count == 0)
			{
				timer2.Stop();

			}
			else
			{
				Tuple<PictureBox, PictureBox> tempA = tempQ.Dequeue();
				ABoxPic_Click(tempA.Item1, new EventArgs());
				ABoxPic_Click(tempA.Item2, new EventArgs());
			}
		}


	}
}

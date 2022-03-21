using System.Windows.Forms;
using System.Drawing;
public class Resolution
{
    float heightRatio = new float();
    float widthRatio = new float();
    int standardHeight, standardWidth;
    public void ResizeForm(Form objForm, int DesignerHeight, int DesignerWidth)
    {
        standardHeight = DesignerHeight;
        standardWidth = DesignerWidth;
        int presentHeight = objForm.Height; //Screen.PrimaryScreen.WorkingArea.Height;//.Bounds.Height;
        int presentWidth = objForm.Width; //Screen.PrimaryScreen.Bounds.Width;
        heightRatio = (float)((float)presentHeight / (float)standardHeight);
        widthRatio = (float)((float)presentWidth / (float)standardWidth);
        objForm.AutoScaleMode = AutoScaleMode.None;
        objForm.Scale(new SizeF(widthRatio, heightRatio));
        foreach (Control c in objForm.Controls)
        {
            if (c.GetType() == typeof(System.Windows.Forms.PictureBox))
            {
                if (c.HasChildren)
                {
                    ResizeControlStore(c);
                }
                else
                {
                    c.Font = new Font("Microsoft Sans Serif", c.Font.SizeInPoints * heightRatio * widthRatio);
                }
            }
        }
        //objForm.Font = new Font("Microsoft Sans Serif", objForm.Font.SizeInPoints * heightRatio * widthRatio);
    }

    private void ResizeControlStore(Control objCtl)
    {
        if (objCtl.HasChildren)
        {
            foreach (Control cChildren in objCtl.Controls)
            {
                if (cChildren.GetType() == typeof(System.Windows.Forms.PictureBox))
                {
                    if (cChildren.HasChildren)
                    {
                        ResizeControlStore(cChildren);
                    }
                    else
                    {
                        cChildren.Font = new Font("Microsoft Sans Serif", cChildren.Font.SizeInPoints * heightRatio * widthRatio);
                    }
                }
            }
            objCtl.Font = new Font("Microsoft Sans Serif", objCtl.Font.SizeInPoints * heightRatio * widthRatio);
        }
        else
        {
            objCtl.Font = new Font("Microsoft Sans Serif", objCtl.Font.SizeInPoints * heightRatio * widthRatio);
        }
    }
}
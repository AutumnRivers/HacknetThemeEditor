using Hacknet.Gui;

using Pathfinder.Executable;

namespace HacknetThemeEditor
{
    public class ExampleExecutable : GameExecutable
    {
        public ExampleExecutable() : base()
        {
            this.baseRamCost = 200;
            this.ramCost = 200;
            this.IdentifierName = "ExampleExec";
            this.name = "ExampleExec";
        }

        public override void Draw(float t)
        {
            drawTarget();
            drawOutline();

            if(Button.doButton(837617, bounds.X + 20, bounds.Y + 20, 50, 50, "Exit", os.defaultHighlightColor))
            {
                needsRemoval = true;
            }
        }
    }
}

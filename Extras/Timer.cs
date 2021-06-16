using System;
using Microsoft.Xna.Framework;

namespace Spacial_Partition.Extras {
   public class Timer : GameComponent {
        //Goes in the Initialize();
        //Timer timer = new Timer(this, 0f);
        //Components.Add(timer);
        //timer.SetTimer(3f, () => { Debug.WriteLine("timer complete."); });

        private Action timerAction;
        public float Delay { get; private set; }
        public float TimeRemaining { get; set; }
        public bool Running { get; private set; }

        public Timer(Game game, float delay) : base(game) {
            TimeRemaining = delay;
            Delay = delay;
            Running = true;
        }

        public void Start() {
            if (TimeRemaining > 0) {
                Running = true;
            }
		}

        public void Reset() {
            TimeRemaining = Delay;
		}

		public void SetTimer(float delay, Action timerAction) {
			TimeRemaining = delay;
			this.timerAction = timerAction;
            Running = true;
		}

		public override void Update(GameTime gameTime) {
           
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (Running) {
                TimeRemaining -= deltaTime;
                
                if (TimeRemaining <= 0) {
                    //TimeRemaining = 0;
                    //Running = false;
                    if (timerAction != null) { 
                        timerAction(); 
                    }
                }
            }
        }
    }
}

using System;
using Spacial_Partition.QuadTrees.Boids;
using Spacial_Partition.QuadTrees.Movers;

namespace Spacial_Partition {
	public static class Program {
		[STAThread]
		static void Main() {
			using (var game = new Game2())
				game.Run();
		}
	}
}

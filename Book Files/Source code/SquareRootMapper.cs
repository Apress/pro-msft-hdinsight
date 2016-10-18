using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Hadoop.MapReduce;

namespace HadoopClient
{
    class SquareRootMapper: MapperBase
    {
        public override void Map(string inputLine, MapperContext context)
        {
            int input = int.Parse(inputLine);

            // Find the square root.
            double root = Math.Sqrt((double)input);

            // Write output.
            context.EmitKeyValue(input.ToString(), root.ToString());
        }
    }
}

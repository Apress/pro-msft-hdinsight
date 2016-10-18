using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Hadoop.MapReduce;

namespace HadoopClient
{
    class SquareRootReducer: ReducerCombinerBase
    {
        public override void Reduce(string key, IEnumerable<string> values, ReducerCombinerContext context)
        {            
            //throw new NotImplementedException();
        }
    }
}

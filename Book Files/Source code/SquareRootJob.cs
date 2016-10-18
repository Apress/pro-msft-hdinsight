using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Hadoop.MapReduce;

namespace HadoopClient
{
    class SquareRootJob: HadoopJob<SquareRootMapper>
    {
        public override HadoopJobConfiguration Configure(ExecutorContext context)
        {
            var config = new HadoopJobConfiguration
            {
                InputPath = Constants.wasbPath + "/example/data/Numbers.txt",
                OutputFolder = Constants.wasbPath + "/example/data/SqaureRootOutput"
            };
            return config;            
        }
    }
}

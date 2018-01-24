/*
 * Copyright 2018 iKoskiX.Lab
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 * http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Threading;

namespace Lode.Web.Strandard.Collections
{
    public class SimpleDisposable : IDisposable
    {
        protected Action OnDispose { get; set; }
        protected int Disposed = 0;

        public SimpleDisposable(Action onDispose)
        {
            OnDispose = onDispose;
        }

        public void Dispose()
        {
            if (Interlocked.Exchange(ref Disposed, 1) == 0)
            {
                OnDispose();
            }
        }

        ~SimpleDisposable()
        {
            Dispose();
        }
    }
}

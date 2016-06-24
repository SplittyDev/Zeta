using System;
namespace libzeta {
    
    /// <summary>
    /// Buffer interface.
    /// </summary>
    public interface IBuffer<T> : IBindable, IPointTo<T> {
        int BufferSize { get; set; }
    }
}


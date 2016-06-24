using System;
namespace libzeta {

    /// <summary>
    /// Pointer interface.
    /// </summary>
    public interface IPointTo<T> {
        void PointTo (T where);
        void PointTo (T where, T offset);
        void PointTo (T where, params T [] other);
    }
}


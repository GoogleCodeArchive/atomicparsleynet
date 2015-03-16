# Transformation Matrix #

Movie files use matrices to describe spatial information about many objects, such as tracks within a movie.

A transformation matrix defines how to map points from one coordinate space into another coordinate space. By modifying the contents of a transformation matrix, you can perform several standard graphics display operations, including translation, rotation, and scaling. The matrix used to accomplish two-dimensional transformations is described mathematically by a 3-by-3 matrix.


All values in the matrix are 32-bit [fixed-point numbers](T_MP4_Fixed_2.md) divided as 16.16, except for the last {u, v, w} column, which contains 32-bit fixed-point numbers divided as 2.30.

> #### How display matrices are used ####
![https://atomicparsleynet.googlecode.com/svn/trunk/MP4/Help/qt_l_054.gif](https://atomicparsleynet.googlecode.com/svn/trunk/MP4/Help/qt_l_054.gif)

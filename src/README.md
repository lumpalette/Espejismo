# Order and accessibility of class members and structure

## Accessibility order

1. Instance API
2. Static API

## Member order

1. Constants
2. Fields
    1. Readonly fields
    2. Regular fields
3. Constructors
4. Finalizers
5. Properties
6. Indexers
7. Events
8. Methods
    1. Base methods
    2. Overriden methods
9. Explicit interface implementation
    1. Properties
    2. Indexers
    3. Methods
10. Operators
11. Nested types
    1. Classes
    2. Structures
    3. Interfaces
    4. Enumerations
    5. Delegates

## Additional notes

*Note:* Explicit members of the same interface must be grouped together 

*Note:* Consider using a partial class when implementing many interfaces.

*Note:* Ensure that nested types are always private.

*Note:* Consider using a partial class and multiple files if one type contains two or more nested types.
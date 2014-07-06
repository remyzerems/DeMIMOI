DeMIMOI Library
=======

Delayed Multiple Input Multiple Output Interface Library

DeMIMOI is a C# library which allows the user to easily build complex connection based systems.
It can be used to model almost anything you can think of as an input/output system.
It can also be seen as an implementation of dynamical systems models in control theory, but can be used in many other contexts.

###Features
* Create multiple input multiple output models easily by only coding the inner function.
* Inputs/Outputs can be numbers as well as any other object
* Natively access past input/output data (nothing to code, just specify how many steps back in time you want to have)
* Collection of models to manage many models at the same time
* Connect/disconnect models to each other to build complex connection based models
* GraphViz code is available for each model to help visualize the models and their connections
* Demos included to show some of the features : Fibonacci sequence, 1st order low pass filter, chained 1st order low pass filter to build a 5th order low pass filter


###Basic example

![ ](/DeMIMOI_1i_F_1o.png?raw=true "DeMIMOI model with 1 input, 1 output")

Imagine that you have to build a (dumb) system that multiplies by 2 its input value.
It would be represented like this : o(t) = 2*i(t) with o(t) outputs at time t, i(t) inputs at time t.

You code what's in the inside of the model (i.e. the function o(t) which creates outputs using the input).
Then just call the update function of the model each time you present the model some new data, or at a specific refresh rate.
You can access the calculated output.
The library also natively manages for you the past input/output data. So you can access the input or outputs at t-3, or t-7 for example.

Now the most interesting part is that you can connect and disconnect on the fly models to each other.
In the example above, you could connect (chain) three identical models, so you would obtain a multiplication by 8 between the input and the output of the chain.

This is a very basic example, but this library can be used for far more complex things like control theory models, signal processing, artificial intelligence...



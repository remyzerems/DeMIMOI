DeMIMOI Library
=======

Delayed Multiple Input Multiple Output Interface Library

DeMIMOI is a C# library which allows the user to easily build complex connection based systems.
It can be used to model almost anything you can think of as an input/output system.
It can also be seen as an implementation of dynamical systems models in control theory, but can be used in many other contexts.
It is also strongly related to the graph theory.
It's more or less close to the Matlab Simulink approach combined to the power of the C# language, with some difference...

### Features
* Create connectable multiple input multiple output models easily by only coding the inner function.
* Dynamically connect/disconnect models to each other to build complex connection based models
* Natively access past input/output data (nothing to code, just specify how many steps back in time you want to have)
* Inputs/Outputs can be numbers as well as any other object (images, instance of a class, arrays...)
* Collection of models to manage many models at the same time (e.g. update all the models using myCollection.UpdateAndLatch())
* Topological order update to update each model following their dependencies ([thank you Martin !](https://github.com/martindevans/TopologicalSorting))
* Connection/Disconnection events available
* GraphViz code is available for each model to help visualize the models and their connections ([See an example](https://aiaddict.files.wordpress.com/2015/12/al5c-sensory-motor-sample-application.png))
* Monitoring windows to directly see your model structure and your models properties
* Windows Form controls mapping support :
  * DeMIMOI_Chart to plot data produced by other models
  * DeMIMOI_PictureBox to display images coming from other models
* Demos included to show some of the features : Fibonacci sequence, 1st order low pass filter, chained 1st order low pass filter to build a 5th order low pass filter


### Basic example

![ ](/DeMIMOI_1i_F_1o.png?raw=true "DeMIMOI model with 1 input, 1 output")

Imagine that you have to build a (dumb) system that multiplies by 2 its input value.
It would be represented like this : o(t) = 2*i(t) with o(t) outputs at time t, i(t) inputs at time t.

You have to code what's in the inside of the model (i.e. the function o(t) which creates outputs using the input).
It would look like this :
```csharp
using DeMIMOI_Models;

class By2Multiplier:DeMIMOI
{
    // Create the multiplier model with 1 input, 2 delayed inputs, 1 output, 1 delayed output
    public By2Multiplier()
        : base(new DeMIMOI_Port(2), new DeMIMOI_Port(1))
    {
        Name = "By 2 Multiplier";

        // Sets the default input/output
        Inputs[0][0].Value = 0.0;
        Outputs[0][0].Value = 0.0;
    }
    
    // This function is called by the DeMIMOI model when asked to update the outputs
    protected override void UpdateInnerSystem(ref DeMIMOI_InputOutput[] new_outputs)
    {
        // Multiply the input by two
        new_outputs[0].Value = ((double)Inputs[0][0].Value) * 2;
    }
}
```

Then just call the update function of the model each time you present the model some new data, or at a specific refresh rate.
```csharp
By2Multiplier myBy2Multiplier = new By2Multiplier();
// Set the input to 5.0
myBy2Multiplier.Inputs[0][0].Value = 5.0;
// Update the multiplier and latch the outputs
myBy2Multiplier.UpdateAndLatch();
```
You can access the calculated output.
```csharp
double result = (double)myBy2Multiplier.Outputs[0][0].Value
// Here result = 10.0
```

The library also natively manages for you the past input/output data. So you can access the input or outputs at t-3, or t-7 for example.
```csharp
double previous_value = (double)myBy2Multiplier.Inputs[0][1].Value
// Here previous_value = 5.0 (i.e. the value Inputs[0][0].Value had at t-1)
```

Now the most interesting part is that you can connect and disconnect on the fly models to each other.
In the example above, you could connect (chain) three identical models, so you would obtain a multiplication by 8 between the input and the output of the chain.
```csharp
By2Multiplier myBy2Multiplier0 = new By2Multiplier();
By2Multiplier myBy2Multiplier1 = new By2Multiplier();
By2Multiplier myBy2Multiplier2 = new By2Multiplier();

// Connect the second model input to the first model output
myBy2Multiplier1.Inputs[0][0].ConnectTo(ref myBy2Multiplier0.Outputs[0][0]);
// Connect the third model input to the second model output
myBy2Multiplier2.Inputs[0][0].ConnectTo(ref myBy2Multiplier1.Outputs[0][0]);

// Set the input to 5.0
myBy2Multiplier0.Inputs[0][0].Value = 5.0;
// Update the filters and latch the outputs
myBy2Multiplier0.UpdateAndLatch();
myBy2Multiplier1.UpdateAndLatch();
myBy2Multiplier2.UpdateAndLatch();

double result = (double)myBy2Multiplier2.Outputs[0][0].Value
// Here result = 40.0
```

This is a very basic example, but this library can be used for far more complex things like control theory models, signal processing, artificial intelligence...

### Use case
For a more detailed view of what you can do with this library, check this website : https://aiaddict.wordpress.com/

# Radial Fog Renderer Feature for Unity

## Overview

The **Radial Fog Renderer Feature** for Unity enhances the built-in fog system by accurately calculating the real-world distance from the camera to the objects in the scene. Unlike Unity's default fog, which can cause inconsistencies by using the depth texture, this system ensures a seamless fog effect across the entire screen, regardless of object position. This results in more realistic and consistent fog rendering, improving the atmospheric quality of your game scenes.

Standard Fog:

![StandardFog](https://github.com/Alexander-Koutrakis/Radial-Fog/assets/61294700/e4c43d5f-75b6-4492-b65f-58ddfde15b8c)

Radial Fog:

![RadialFog](https://github.com/Alexander-Koutrakis/Radial-Fog/assets/61294700/ff378260-07a1-4a9c-a00e-05e59159f09e)

Standard/Radial

![StandardFog](https://github.com/Alexander-Koutrakis/Radial-Fog/assets/61294700/82fc4bd6-fb6c-4e33-a88d-f827bd3bdfc6)       ![RadialFog](https://github.com/Alexander-Koutrakis/Radial-Fog/assets/61294700/5173395f-4921-457a-bd85-ceb384d1fd9d)



### How It Works

1. **Frustum Corners**:
   - Using `Camera.CalculateFrustumCorners`, the system determines the rays for the corners of the camera's frustum. These rays are then interpolated to calculate distances for all pixels, ensuring accurate fog application across the entire screen.

2. **Ray Calculation**:
   - For each pixel, a ray is cast from the camera to the far plane. If an object is hit, the depth value is less than 1, indicating the distance at which the object was encountered.

3. **Real-World Distance**:
   - By scaling the initial ray based on the depth value, the system calculates the actual distance from the camera to the object.


This advanced calculation ensures that objects blend seamlessly into the fog as they should, providing a more immersive and visually appealing experience.

## Components

### Settings Class
Holds the configuration parameters for the fog effect, including fog mode, start and end distances, density, and color.

### RadialFogPass Class
Manages the rendering logic for the radial fog, calculating frustum corners and applying the fog effect based on the settings.

### RadialFogFeature Class
Integrates the radial fog effect into the Universal Render Pipeline by adding the necessary render pass.

### Fog Shader
Implements the fog rendering logic, using different algorithms for linear, exponential, and exponential squared fog effects to blend the fog color with the scene based on depth.

## Usage

1. **Copy the RadialFog Folder**:
   - Copy the contents of the `RadialFog` folder to your Unity project.

2. **Add the Radial Fog Feature**:
   - Open your URP Renderer asset (e.g., `ForwardRenderer.asset`).
   - In the **Inspector** window, click on **Add Renderer Feature** and select **Radial Fog Feature**.
   - Adjust the fog settings according to your requirements.

## Additional Notes

- The Radial Fog system works with both Deferred and Forward rendering paths.
- Since it relies on the `CameraDepthTexture`, any material that doesn't write to the depth buffer will be ignored, meaning transparent objects won't be affected by the fog.
 

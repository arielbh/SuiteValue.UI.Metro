# SuiteValue.UI.Metro

SuiteValue.UI.Metro is a framework targeting WinRT XAML/C# Metro development, and it helps develop various tasks common for Metro apps.
## What’s in the Box

1.  Fully pluggable and open providers system for authentications. It wraps around WebAuthenticationBroker and offers common user info details from each service. Out of the box support for Google, Facebook, Twitter and LiveID.
2.  Very simple and naive implementation of Time and Date pickers.
3.  BaseViewModels which help navigation and async scenarios. 


## Using the framework

There are nuget packages you can consume that will help you get started.
Also Samples are available
	A sample showing how to use Time/Date Pickers.
	A sample showing how to use the authentication feature.

More sample will be available in the future.

To compile the framework you must install Live SDK
http://msdn.microsoft.com/en-us/live/ff621310.aspx

## Configuring your app to be used with Twitter
  Follow instructions as specified in this link: https://dev.twitter.com/
  You will need to provide the dynamic conifguration object with 2 keys:
    TwitterClientId - should be available after completing app registration.
    TwitterClientSecret- should be available after completing app registration.
    TwitterRedirectUrl - could be any url.

## Configuring your app to be used with Google

  Follow instructions as specified in this link: https://developers.google.com/accounts/docs/OAuth2
  You will need to provide the dynamic conifguration object with 2 keys:
    GoogleClientId- should be available after completing app registration.
    GoogleClientSecret - should be available after completing app registration.
    GoogleRedirectUrl - should be "urn:ietf:wg:oauth:2.0:oob"


## Configuring your app to be used with Facebook

  Register your app with https://developers.facebook.com/apps
  You will need to provide the dynamic conifguration object with 2 keys:
    FacebookClientId - should be available after completing app registration.
    FacebookRedirectUrl - could be any url.

## Configuring your app to be used with LiveSDK

 Register you Metro app with https://manage.dev.live.com/build?wa=wsignin1.0


## Contact

Contact me at twitter : @arielbh.
I would love to hear your feedback on that.
 


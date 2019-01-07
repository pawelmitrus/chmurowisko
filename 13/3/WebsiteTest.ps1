Configuration WebsiteTest {

    param (
        [Parameter(Mandatory = $False)]
        [String]$websiteUri
    )

    # Import the module that contains the resources we're using.
    Import-DscResource -ModuleName PsDesiredStateConfiguration

    # The Node statement specifies which targets this configuration will be applied to.
    Node 'localhost' {

        # Install IIS features
        WindowsFeature WebServerRole {
            Name   = "Web-Server"
            Ensure = "Present"
        }

        WindowsFeature WebManagementService {
            Name   = "Web-Mgmt-Service"
            Ensure = "Present"
        }

        WindowsFeature ASPNet45 {
            Name   = "Web-Asp-Net45"
            Ensure = "Present"
        }

        WindowsFeature HTTPRedirection {
            Name   = "Web-Http-Redirect"
            Ensure = "Present"
        }

        WindowsFeature CustomLogging {
            Name   = "Web-Custom-Logging"
            Ensure = "Present"
        }

        WindowsFeature LogginTools {
            Name   = "Web-Log-Libraries"
            Ensure = "Present"
        }

        WindowsFeature RequestMonitor {
            Name   = "Web-Request-Monitor"
            Ensure = "Present"
        }

        WindowsFeature Tracing {
            Name   = "Web-Http-Tracing"
            Ensure = "Present"
        }

        WindowsFeature BasicAuthentication {
            Name   = "Web-Basic-Auth"
            Ensure = "Present"
        }

        WindowsFeature WindowsAuthentication {
            Name   = "Web-Windows-Auth"
            Ensure = "Present"
        }

        WindowsFeature ApplicationInitialization {
            Name   = "Web-AppInit"
            Ensure = "Present"
        }

         if (![String]::IsNullOrEmpty($websitePackageUri)) {

            # Download the website into the default website
            Script DeployWebPackage {
                GetScript  = {@{Result = "DeployWebPackage"}}
                TestScript = {
                    return Test-Path -Path "C:\WebApp\site.html";
                }
                SetScript  = {

                    if (!(Test-Path -Path "C:\WebApp")) {
                        New-Item -Path "C:\WebApp" -ItemType Directory -Force | Out-Null;
                    }

                    $dest = "C:\WebApp\site.html" 

                    Invoke-WebRequest -Uri $using:websitePackageUri -OutFile $dest -UseBasicParsing;

                    Move-Item -Path $dest -Destination "C:\inetpub\wwwroot" -Force;
                }
                DependsOn  = "[WindowsFeature]WebServerRole"
            }
        }
    }
}
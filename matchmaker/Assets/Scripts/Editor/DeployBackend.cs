using GPFEditor.Settings;
using GPFEditor.AwsTypes;
using UnityEditor;
using UnityEngine;
using System;

public static class DeployBackend
{
    static void Deploy()
    {
        DeployFromEnv();
    }

    [MenuItem("BUILD/Deploy")]
    static void DeployFromEditor()
    {
        DeployInternal("gpf-react",
            "us-west-2",
            "YMGZC89555B4HZ68ZHHFU4V4W2",
            "gameplumbers.com",
            "gpf-react",
            "arn:aws:acm:us-west-2:525665374615:certificate/5d0d5e32-cc7c-4f13-9406-880c4674f3d8",
            "Z0546205385NZNV5ZUZ65");
    }

    static void DeployFromEnv()
    {
        var stackName = System.Environment.GetEnvironmentVariable("STACK_NAME");
        var awsRegion = System.Environment.GetEnvironmentVariable("AWS_REGION");
        var apiKey = System.Environment.GetEnvironmentVariable("API_KEY");
        var domainName = System.Environment.GetEnvironmentVariable("DOMAIN_NAME");
        var subdomain = System.Environment.GetEnvironmentVariable("SUBDOMAIN");
        var certArn = System.Environment.GetEnvironmentVariable("CERT_ARN");
        var hostedZoneId = System.Environment.GetEnvironmentVariable("HOSTED_ZONE_ID");

        DeployInternal(stackName,
            awsRegion,
            apiKey,
            domainName,
            subdomain,
            certArn,
            hostedZoneId);
    }

    static async void DeployInternal(string stackName,
            string awsRegion,
            string apiKey,
            string domainName,
            string subdomain,
            string certArn,
            string hostedZoneId)
    {
        try
        {
            var userSettings = GPFSettings.userSettings;
            userSettings.aws.profile = null;
            userSettings.aws.apiKey = apiKey;

            var stackId = new StackIdentifier(stackName, awsRegion);

            GpfServerStackSettings stackSettings = new GpfServerStackSettings();

            stackSettings.identifier = stackId;
            stackSettings.clearable = true;

            stackSettings.HostedZoneId = hostedZoneId;
            stackSettings.Domain = $"{subdomain}.{domainName}";
            stackSettings.Subdomain = subdomain;
            stackSettings.Cert = certArn;
            stackSettings.UseDNS = true;

            await GpfServerDeployStack.Deploy(stackSettings);
        }
        catch(Exception e)
        {
            Debug.LogException(e);
            if (Application.isBatchMode)
                EditorApplication.Exit(1);
        }
        finally
        {
            if (Application.isBatchMode)
                EditorApplication.Exit(0);
        }
    }
}

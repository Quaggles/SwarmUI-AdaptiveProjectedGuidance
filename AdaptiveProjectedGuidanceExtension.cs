using System.IO;
using Newtonsoft.Json.Linq;
using SwarmUI.Builtin_ComfyUIBackend;
using SwarmUI.Core;
using SwarmUI.Text2Image;
using SwarmUI.Utils;

namespace Quaggles.Extensions.AdaptiveProjectedGuidance;

public class AdaptiveProjectedGuidanceExtension : Extension
{
    private const string Prefix = "[APG] ";
    private const string FeatureId = "adaptiveprojectedguidance";

    public T2IRegisteredParam<double> Momentum,
        AdaptiveMomentum,
        NormThreshold,
        Eta,
        GuidanceSigmaStart,
        GuidanceSigmaEnd;

    public T2IRegisteredParam<bool> GuidanceLimiter;

    public override void OnInit()
    {
        base.OnInit();

        // Add the JS file, which manages the install buttons for the comfy nodes
        ScriptFiles.Add("assets/apg.js");

        // Define required nodes
        ComfyUIBackendExtension.NodeToFeatureMap["APG_ImYourCFGNow"] = FeatureId;

        // Add required custom node as installable feature
        InstallableFeatures.RegisterInstallableFeature(
            new(
                "APG_ImYourCFGNow",
                FeatureId,
                "https://github.com/MythicalChu/ComfyUI-APG_ImYourCFGNow",
                "MythicalChu",
                "This will install the APG_ImYourCFGNow ComfyUI node developed by MythicalChu.\nDo you wish to install?"
            )
        );

        // Prevents install button from being shown during backend load if it looks like it was installed
        // it will appear if the backend loads and the backend reports it's not installed
        if (
            Directory.Exists(
                Utilities.CombinePathWithAbsolute(
                    Environment.CurrentDirectory,
                    $"{ComfyUIBackendExtension.Folder}/DLNodes/APG_ImYourCFGNow"
                )
            )
        )
        {
            ComfyUIBackendExtension.FeaturesSupported.UnionWith([FeatureId]);
            ComfyUIBackendExtension.FeaturesDiscardIfNotFound.UnionWith([FeatureId]);
        }

        T2IParamGroup paramGroup = new(
            "Adaptive Projected Guidance",
            Toggles: true,
            Open: false,
            IsAdvanced: false,
            OrderPriority: 9
        );
        int orderCounter = 0;
        Momentum = T2IParamTypes.Register<double>(
            new(
                $"{Prefix}Momentum",
                "Use this to control how saturated/burned your images are",
                "0.5",
                Min: -1.5,
                Max: 1,
                Step: 0.05,
                ViewType: ParamViewType.SLIDER,
                Group: paramGroup,
                FeatureFlag: FeatureId,
                OrderPriority: orderCounter++
            )
        );
        AdaptiveMomentum = T2IParamTypes.Register<double>(
            new(
                $"{Prefix}Adaptive Momentum",
                "Gradually brings momentum towards 0 every step"
                    + "\n0 = No change to momentum"
                    + "\n0.18 = Momentum will reach 0 roughly around the last step. This can help to dramatically lower glitches/noise, specially on lower steps or higher CFG values. This is the default value"
                    + "\n0.19 = Momentum will reach 0 roughly around half the steps",
                "0.18",
                Min: 0,
                Max: 1,
                Step: 0.001,
                ViewType: ParamViewType.SLIDER,
                Group: paramGroup,
                FeatureFlag: FeatureId,
                OrderPriority: orderCounter++
            )
        );
        NormThreshold = T2IParamTypes.Register<double>(
            new(
                $"{Prefix}Norm Threshold",
                "",
                "15",
                Min: 0,
                Max: 50,
                Step: 0.5,
                ViewType: ParamViewType.SLIDER,
                Group: paramGroup,
                FeatureFlag: FeatureId,
                OrderPriority: orderCounter++
            )
        );
        Eta = T2IParamTypes.Register<double>(
            new(
                $"{Prefix}Eta",
                "",
                "1",
                Min: 0,
                Max: 1,
                Step: 0.1,
                ViewType: ParamViewType.SLIDER,
                Group: paramGroup,
                FeatureFlag: FeatureId,
                OrderPriority: orderCounter++
            )
        );
        GuidanceSigmaStart = T2IParamTypes.Register<double>(
            new(
                $"{Prefix}Guidance Sigma Start",
                "",
                "5.42",
                Min: -1,
                Max: 10000,
                Step: 0.01,
                ViewType: ParamViewType.SLIDER,
                Group: paramGroup,
                FeatureFlag: FeatureId,
                OrderPriority: orderCounter++
            )
        );
        GuidanceSigmaEnd = T2IParamTypes.Register<double>(
            new(
                $"{Prefix}Guidance Sigma End",
                "",
                "0.28",
                Min: -1,
                Max: 10000,
                Step: 0.01,
                ViewType: ParamViewType.SLIDER,
                Group: paramGroup,
                FeatureFlag: FeatureId,
                OrderPriority: orderCounter++
            )
        );
        GuidanceLimiter = T2IParamTypes.Register<bool>(
            new(
                $"{Prefix}Guidance Limiter",
                "",
                "false",
                Group: paramGroup,
                FeatureFlag: FeatureId,
                OrderPriority: orderCounter++
            )
        );
        WorkflowGenerator.AddModelGenStep(
            g =>
            {
                // Required param
                if (!g.UserInput.TryGet(Momentum, out var momentum))
                    return;

                if (!g.Features.Contains(FeatureId))
                    throw new SwarmUserErrorException(
                        "AdaptiveProjectedGuidance parameters specified, but 'APG_ImYourCFGNow' feature isn't installed"
                    );

                string nodeId = g.CreateNode(
                    "APG_ImYourCFGNow",
                    new JObject
                    {
                        ["model"] = g.LoadingModel,
                        ["momentum"] = momentum,
                        ["adaptive_momentum"] = g.UserInput.Get(AdaptiveMomentum),
                        ["norm_threshold"] = g.UserInput.Get(NormThreshold),
                        ["eta"] = g.UserInput.Get(Eta),
                        ["guidance_sigma_start"] = g.UserInput.Get(GuidanceSigmaStart),
                        ["guidance_sigma_end"] = g.UserInput.Get(GuidanceSigmaEnd),
                        ["guidance_limiter"] = g.UserInput.Get(GuidanceLimiter),
                        ["print_data"] = false,
                    }
                );

                g.FinalModel = [nodeId, 0];
                g.LoadingModel = [nodeId, 0];
            },
            -13
        );
    }
}

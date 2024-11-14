postParamBuildSteps.push(() => {
    let group = document.getElementById('input_group_content_adaptiveprojectedguidance');
    if (group) {
        if (!currentBackendFeatureSet.includes('adaptiveprojectedguidance')) {
            group.append(createDiv(`adaptiveprojectedguidance_install_button`, 'keep_group_visible', `<button class="basic-button" onclick="installFeatureById('adaptiveprojectedguidance', 'adaptiveprojectedguidance_install_button')">Install APG_ImYourCFGNow</button>`));
        }
    }
});

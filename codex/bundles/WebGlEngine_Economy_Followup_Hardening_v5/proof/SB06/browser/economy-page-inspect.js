async page => {
    return {
        url: page.url(),
        title: await page.title(),
        bodyText: (await page.locator("body").innerText({ timeout: 5000 }).catch(error => String(error))).slice(0, 2000)
    };
}

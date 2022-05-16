import * as Unity from "./unity"
window.gpfReact.userId = "user"
window.gpfReact.appId = "main"
window.gpfReact.analytics = {}
window.gpfReact.analytics.promise = new Promise((resolve, reject) => {
  window.gpfReact.analytics.reject = reject
  window.gpfReact.analytics.resolve = resolve
})
export async function Load(appId, userId, abTestListener) {
  window.gpfReact.userId = userId
  window.gpfReact.appId = appId
  window.gpfReact.userSoid = "analytics/" + userId
  window.gpfReact.abtestSoid = "ab_tests/" + appId
  await Unity.Load(so => {
    if (so.ID.id === window.gpfReact.abtestSoid) abTestListener(so.Tests)
  })
    .then(() => {
      Unity.Sync(window.gpfReact.abtestSoid)
      Unity.Sync(window.gpfReact.userSoid)
      window.gpfReact.analytics.resolve()
    })
    .catch(message => {
      console.error(message)
      window.gpfReact.analytics.reject()
    })
}

export function SendAnalytics(evtAttributes) {
  window.gpfReact.analytics.promise.then(() => {
    Unity.Send(window.gpfReact.userSoid, "AnalyticsUserSO+Message", {
      evtAttributes,
    })
  })
}

export function SetTest(testName, testValue) {
  window.gpfReact.analytics.promise.then(() => {
    Unity.Send(window.gpfReact.abtestSoid, "ABTestsSO+SetTest", {
      name: testName,
      value: testValue,
    })
  })
}

export function RemoveTest(testName) {
  window.gpfReact.analytics.promise.then(() => {
    Unity.Send(window.gpfReact.abtestSoid, "ABTestsSO+RemoveTest", {
      name: testName,
    })
  })
}

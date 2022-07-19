import * as React from "react"
import { Link, graphql } from "gatsby"
import { Provider } from "react-redux"
import store from "./store"
import {
  Load,
  RemoveTest,
  SendAnalytics,
  SetTest,
  SetEntry,
  GetLeaderboardEntries,
} from "./analyticsWrapper"

import Bio from "../components/bio"
import Layout from "../components/layout"
import Seo from "../components/seo"

let appId = "main"
let userId =
  appId + "-" + Math.floor(Math.random() * 100000000) + "-" + Date.now()
Load(appId, userId, test => {
  console.log(test)
})
//SendAnalytics({ action: "test action" })
SetTest("test1", "A")
SetTest("test1", "B")
SetTest("test1", "C")
SetTest("test2", "test")
SetTest("test3", "deleteMe")
RemoveTest("test3")

function TestLeaderboard() {
  SetEntry("coin_player/test_id_1", "Carrie", 2)
  SetEntry("coin_player/test_id_2", "Gene", 4)
  SetEntry("coin_player/test_id_3", "Kelly", 7)

  setTimeout(async () => {
    var entries = await GetLeaderboardEntries()
    console.log(entries)
  }, 5000)
}
TestLeaderboard()

const BlogIndex = ({ data, location }) => {
  const siteTitle = data.site.siteMetadata?.title || `Title`

  return (
    <Provider store={store}>
      <Layout location={location} title={siteTitle}>
        <canvas id="unity-canvas" width="100%" height="100%"></canvas>
        <Seo title="All posts" />
        <Bio />
      </Layout>
    </Provider>
  )
}

export default BlogIndex

export const pageQuery = graphql`
  query {
    site {
      siteMetadata {
        title
      }
    }
    allMarkdownRemark(sort: { fields: [frontmatter___date], order: DESC }) {
      nodes {
        excerpt
        fields {
          slug
        }
        frontmatter {
          date(formatString: "MMMM DD, YYYY")
          title
          description
        }
      }
    }
  }
`

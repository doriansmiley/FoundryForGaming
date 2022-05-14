import * as React from "react"
import { Link, graphql } from "gatsby"
import { Provider } from "react-redux"
import store from "./store"
import { Load, Sync, Unsync, Send } from "./unity"

import Bio from "../components/bio"
import Layout from "../components/layout"
import Seo from "../components/seo"

function delay(time) {
  return new Promise(resolve => setTimeout(resolve, time))
}

Load(so => {
  console.log(so.ID.id)
  store.dispatch({ type: "SET_SO_STATE", so: so })
})
Sync("analytics/test_counter")
Send("analytics/test_counter", "AnalyticsUserSO+Message", {})
Send("analytics/test_counter", "AnalyticsUserSO+Message", {})

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

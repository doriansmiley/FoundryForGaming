import * as React from "react"
import { Link, graphql } from "gatsby"
import { Provider } from "react-redux"
import store from "./store"
import unity from "./unity"

import Bio from "../components/bio"
import Layout from "../components/layout"
import Seo from "../components/seo"
import * as types from "./appActionTypes"

// HACK: Make reduce available to Unity
window.reduxStore = store

function delay(time) {
  return new Promise(resolve => setTimeout(resolve, time))
}

delay(1000).then(() => store.dispatch({ type: types.TEST }))

const BlogIndex = ({ data, location }) => {
  const siteTitle = data.site.siteMetadata?.title || `Title`
  const posts = data.allMarkdownRemark.nodes

  if (posts.length === 0) {
    return (
      <Provider store={store}>
        <Layout location={location} title={siteTitle}>
          <Seo title="All posts" />
          <Bio />
          <p>
            No blog posts found. Add markdown posts to "content/blog" (or the
            directory you specified for the "gatsby-source-filesystem" plugin in
            gatsby-config.js).
          </p>
        </Layout>
      </Provider>
    )
  }

  return (
    <Provider store={store}>
      <Layout location={location} title={siteTitle}>
        <canvas id="unity-canvas" width="100%" height="100%"></canvas>
        <Seo title="All posts" />
        <Bio />
        <ol style={{ listStyle: `none` }}>
          {posts.map(post => {
            const title = post.frontmatter.title || post.fields.slug

            return (
              <li key={post.fields.slug}>
                <article
                  className="post-list-item"
                  itemScope
                  itemType="http://schema.org/Article"
                >
                  <header>
                    <h2>
                      <Link to={post.fields.slug} itemProp="url">
                        <span itemProp="headline">{title}</span>
                      </Link>
                    </h2>
                    <small>{post.frontmatter.date}</small>
                  </header>
                  <section>
                    <p
                      dangerouslySetInnerHTML={{
                        __html: post.frontmatter.description || post.excerpt,
                      }}
                      itemProp="description"
                    />
                  </section>
                </article>
              </li>
            )
          })}
        </ol>
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

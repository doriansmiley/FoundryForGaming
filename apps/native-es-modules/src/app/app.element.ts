import './app.element.css';
import { useButton, register } from 'lotusjs-components/lib';
import {
  init,
  JSONObject,
  RemoveTest,
  SendAnalytics,
  SetTest,
} from '@foundry-for-gaming/common-no-react';

export class AppElement extends HTMLElement {
  public static observedAttributes = [];

  connectedCallback() {
    function onSync(value: JSONObject) {
      // this global dync function requires introspection if the value to determine
      // what object was updates. Future version of sync will allow you to pass the
      // function as a param. globalThis.gpfReact?.userSoid and globalThis.gpfReact.abtestSoid
      // hold the ID value for analytics and A/B test objects respectively
      const id = value.ID.id;
      switch (id) {
        case globalThis.gpfReact.abtestSoid:
        case globalThis.gpfReact?.userSoid:
          console.log(`Index.onSync: ${JSON.stringify(value)}`);
          break;
        default:
          console.log(`Index.onSync: no matching object found for id: ${id}`);
      }
    }

    function onABTest(value: JSONObject) {
      console.log(`Index.onABTest: ${JSON.stringify(value)}`);
    }
    const appId = 'main';
    const userId =
      appId + '-' + Math.floor(Math.random() * 100000000) + '-' + Date.now();
    // TODO pass in abTestListener and soListener and show updated on screen
    // when the new leaderboard SO is created hook up and show adding a new score
    // and show the scores changing in real time as the result of game play
    init(appId, userId, 'assets/unity', onABTest, onSync)
      .then((result) => {
        console.log(`result: ${result}`);
        SendAnalytics({ action: 'test action' });
        SetTest('test1', 'C');
        SetTest('test2', 'test');
        SetTest('test3', 'deleteMe');
        RemoveTest('test3');
      })
      .catch((e) => {
        console.log(e);
      });

    const title = 'native-es-modules';
    const template = document.createElement('div');
    template.innerHTML =
      '<template id="app">\n' +
      '  <div data-component-root="root">\n' +
      '    <button data-skin-part="button">\n' +
      '      <label>Hello World with Bootsrap</label>\n' +
      '    </button>\n' +
      '  </div>\n' +
      '</template>\n';
    // create custom tag definitions for Lotus
    const tagDef = {
      inserted: (component) => {
        console.log('example component inserted');
      },
      removed: (component) => {
        console.log('example component removed');
        component.element = null;
      },
      template: template.firstChild as HTMLTemplateElement,
      tagName: 'lotus-button',
      hydrated: false,
      tagFunction: useButton,
    };
    const tagDef2 = {
      inserted: (component) => {
        console.log('example component inserted');
      },
      removed: (component) => {
        console.log('example component removed');
        component.element = null;
      },
      templateUrl: 'assets/templates/button-blue.html',
      tagName: 'lotus-button-2',
      hydrated: false,
      tagFunction: useButton,
    };
    const tagDef3 = {
      inserted: (component) => {
        console.log('example component inserted');
      },
      removed: (component) => {
        console.log('example component removed');
        component.element = null;
      },
      tagName: 'lotus-button-inline',
      hydrated: false,
      tagFunction: useButton,
    };
    // register custom elements with Lotus
    register(tagDef);
    register(tagDef2);
    register(tagDef3);
    this.innerHTML = `
    <div class="wrapper">
      <div class="container">
      <h3>Lotus Button Component</h3>
      <div class="container">
        <h3>Lotus Button</h3>
        <p>
        Here's an example of a Lotus Button component. The <code >lotus-button</code> tag
        extends the <code >HTMLElement</code>. In this example we use the tadDef to define the HTML view as a string.
        Lotus also provides a render method of you want to render programmatically under utils.
        We assign an event listener in to mediate events.
        </p>
        <lotus-button></lotus-button>
      </div>
      <div class="container">
        <h3>Lotus Button Inline</h3>
        <p>
        Here's an example of a Lotus Button component rendered using inline HTML. This is perhaps the most common
        use case as it allows HTML to be defined inline but still seperate from the code.
        </p>
        <lotus-button-inline id="inline-button">
          <template>
              <style>
        @font-face {
        font-family: 'SegoeUI';
        src: url('../assets/fonts/segoeui.ttf') format('truetype');

        font-weight: normal;
        font-style: normal;
        }

        @font-face {
        font-family: 'SegoeUI';
        src: url('../assets/fonts/segoeuib.ttf') format('truetype');

        font-weight: 500;
        font-style: normal;
        }

        @font-face {
        font-family: 'SegoeUI';
        src: url('../assets/fonts/segoeuii.ttf') format('truetype');

        font-weight: normal;
        font-style: italic;
        }


        button{
        cursor:pointer;
        color:#fff;
        width:190px;
        height:50px;
        border-radius:26px;
        box-shadow:-3px 3px 5px rgba(0,0,0,0.3);
        border: 1px solid #02fa0f;

        padding:10px;
        /* Permalink - use to edit and share this gradient: http://colorzilla.com/gradient-editor/#5bb0d8+0,1e5799+44,1e5799+89,1f6b8e+100 */
        background: #5df86b; /* Old browsers */
        background: -moz-linear-gradient(top, #a8d85b 0%, #51991e 44%, #1e9939 89%, #1f8e3b 100%); /* FF3.6-15 */
        background: -webkit-linear-gradient(top, #a8d85b 0%, #51991e 44%, #1e9939 89%, #1f8e3b 100%); /* Chrome10-25,Safari5.1-6 */
        background: linear-gradient(to bottom, #a8d85b 0%, #51991e 44%, #1e9939 89%, #1f8e3b 100%); /* W3C, IE10+, FF16+, Chrome26+, Opera12+, Safari7+ */
        filter: progid:DXImageTransform.Microsoft.gradient( startColorstr='#5bb0d8', endColorstr='#1f6b8e',GradientType=0 ); /* IE6-9 */
        }

        button * {
        cursor:pointer;
        }

        button:focus {
        outline: none;
        }

        button:hover {
        border:1px solid #f00;
        /* Permalink - use to edit and share this gradient: http://colorzilla.com/gradient-editor/#f9c67a+1,fc7e1e+49,f9af02+87,ffcc68+100 */
        background: #f9c67a; /* Old browsers */
        background: -moz-linear-gradient(top, #f9c67a 1%, #fc7e1e 49%, #f9af02 87%, #ffcc68 100%); /* FF3.6-15 */
        background: -webkit-linear-gradient(top, #f9c67a 1%,#fc7e1e 49%,#f9af02 87%,#ffcc68 100%); /* Chrome10-25,Safari5.1-6 */
        background: linear-gradient(to bottom, #f9c67a 1%,#fc7e1e 49%,#f9af02 87%,#ffcc68 100%); /* W3C, IE10+, FF16+, Chrome26+, Opera12+, Safari7+ */
        filter: progid:DXImageTransform.Microsoft.gradient( startColorstr='#f9c67a', endColorstr='#ffcc68',GradientType=0 ); /* IE6-9 */
        }

        button:hover label {
        color:#b71a00;
        text-shadow: 0px 0px 6px rgba(255, 189, 104, 1);
        }

        button label {
        font-family: 'SegoeUI';
        text-shadow: -1px 1px 4px rgba(0, 0, 0, 0.5);
        font-size:14px;
        }
        </style>
              <div data-component-root="root">
              <button data-skin-part="button"><label>testButton inline</label></button>
              </div>
           </template>
         </lotus-button-inline>
      </div>
      <div class="container">
        <h3>Lotus Button with External Skin</h3>
        <p>
        Here's an example of a Lotus Button component using an external skin that is loaded at runtime.
        The tag definition in <code >app.js</code> references the <code >templates/button-blue.html</code> template file.
        The file is static and will only be loaded once no matter how many instances of the tag you have.
        The <code >lotus-button</code> tag
        extends <code >HTMLElement</code>. The component registry adds the component property
        which we use to mediate the component view. We assign an event listener in to mediate events.
        assign and event handler to mediate the component's click event.
        </p>
        <lotus-button-2></lotus-button-2>
      </div>
      </div>
    </div>
      `;
  }
}

customElements.define('foundry-for-gaming-root', AppElement);

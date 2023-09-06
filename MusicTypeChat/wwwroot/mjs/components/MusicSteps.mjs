// SpotifyCommandVisualizer.mjs
import { ref, computed } from 'vue';

export default {
    template: /*html*/ `
    <div class="p-4 bg-gray-100 dark:bg-gray-800 rounded-md">
      <h1 class="text-xl font-semibold mb-4">Spotify Command Visualizer</h1>

      <h2 class="text-lg font-medium mb-2">Steps</h2>
      <ul class="mb-4">
        <li v-for="(step, index) in formattedSteps" :key="index" class="mb-2">
          <div class="p-3 bg-white dark:bg-black rounded-md">
            {{ step }}
          </div>
        </li>
      </ul>

      <h2 class="text-lg font-medium mt-4 mb-2">Tracks</h2>
<ul class="mb-4">
  <li v-for="track in getLastTrackArray(stepResults)" :key="track.uri" class="mb-2">
    <div class="p-3 bg-white dark:bg-black rounded-md">
      <a :href="track.uri" target="_blank" rel="noopener noreferrer" class="text-blue-500 hover:text-blue-700 hover:underline inline-flex items-center">
        {{ track.name }} - {{ track.album }}
        <svg xmlns="http://www.w3.org/2000/svg" class="h-4 w-4 ml-1" fill="none" viewBox="0 0 24 24" stroke="currentColor">
          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1-2V9"></path>
          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M15 13l-3 3m0 0l-3-3m3 3V10"></path>
        </svg>
      </a>
    </div>
  </li>
</ul>

    </div>
  `,
    props: ['response'],
    setup(props) {
        const stepResults = ref(props.response.stepResults);
        const steps = ref(props.response.steps);

        const formattedSteps = computed(() => {
            return steps.value.map((step) => {
                const func = step['@func'];
                const args = step['@args'].map((arg) => {
                    if (typeof arg === 'object' && arg['@ref'] !== undefined) {
                        return `Result[${arg['@ref']}]`;
                    }
                    return JSON.stringify(arg);
                }).join(', ');

                return `${func}(${args})`;
            });
        });

        function getLastTrackArray(stepResults) {
            for (let i = stepResults.length - 1; i >= 0; i--) {
                if (Array.isArray(stepResults[i])) {
                    return stepResults[i];
                }
            }
            return [];
        }

        return { stepResults, formattedSteps,getLastTrackArray };
    },
};
